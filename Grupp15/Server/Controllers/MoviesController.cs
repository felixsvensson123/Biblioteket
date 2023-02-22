using Grupp15.Server.Data;
using Grupp15.Shared.Helpers;
using Grupp15.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Grupp15.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : Controller
    {
        private readonly AuthDbContext _context;

        public MoviesController(AuthDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<MovieModel>> GetMovieList()
        {
            List<MovieModel> dbMovies = await _context.Products.OfType<MovieModel>().ToListAsync();
            return dbMovies;
        }

        [HttpGet("{id}")]
        public async Task<MovieModel> GetMovie(int id)
        {
            var dbMovie = await GetMovieList();
            return dbMovie.SingleOrDefault(b => b.Id == id);
        }

        [HttpPost("bulkadd")]
        public async Task<IActionResult> BulkAdd()
        {
            var file = HttpContext.Request.Form.Files.ToList().Find(file => Path.GetExtension(file.FileName) == ".csv");

            if (file == null)
            {
                return BadRequest();
            }
            else
            {
                var newMovies = CSVHelper.FromFile<MovieModel>(file);

                if (newMovies != null)
                {
                    var movies = await _context.Products.OfType<MovieModel>().ToListAsync();

                    foreach (var movie in movies)
                    {
                        if (newMovies.Any(x => x.Name == movie.Name))
                        {
                            newMovies.RemoveAll(x => x.Name == movie.Name);
                        }
                    }

                    await _context.Products.AddRangeAsync(newMovies);

                    await _context.SaveChangesAsync();
                }

                return Ok();
            }
        }

        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> EditBook([FromBody] MovieModel movie, int id)
        {
            var movies = await _context.Products.OfType<MovieModel>().FirstOrDefaultAsync(x => x.Id == id);
            if (movies != null)
            {
                movies.Director = movie.Director;
                movies.Genre = movie.Genre;
                movies.Description = movie.Description;
                movies.Name = movie.Name;
                movies.Count = movie.Count;

                _context.Update(movies);
                _context.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var movie = await _context.Products.OfType<MovieModel>().FirstOrDefaultAsync(i => i.Id == id);

            if (movie != null)
            {
                _context.Products.Remove(movie);
                _context.SaveChanges();
                return Ok();
            }
            
            return BadRequest();
        }
        private async Task<ApplicationUser?> GetUser(HttpContext context)
        {
            return await _context.Users.FirstAsync(u => u.Email.ToLower() == context.User.Identity!.Name!.ToLower()) ?? null;
        }
    }
}
