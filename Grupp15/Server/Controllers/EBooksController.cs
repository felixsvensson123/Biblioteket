using Grupp15.Server.Data;
using Grupp15.Shared.Helpers;
using Grupp15.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Grupp15.Server.Extensions;

namespace Grupp15.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EBooksController : Controller
    {

        private readonly AuthDbContext _context;

        public EBooksController(AuthDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<EBookModel>> GetAllEBooks()
        {
            List<EBookModel> list = await _context.Products.OfType<EBookModel>().ToListAsync();
            return list;
        }

        [HttpGet("{id}")]
        public async Task<EBookModel> GetEbook(int id)
        {
            var ebooks = await GetAllEBooks();

            return ebooks.SingleOrDefault(x => x.Id == id);
        }
        [Authorize]
        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> EditEBook([FromBody] EBookModel ebook, int id)
        {
            var ebooks = await _context.Products.OfType<EBookModel>().FirstOrDefaultAsync(x => x.Id == id);
            if (ebooks != null)
            {
                ebooks.EAuthor = ebook.EAuthor;
                ebooks.EPages = ebook.EPages;
                ebooks.Name = ebook.Name;
                ebooks.Description = ebook.Description;

                _context.Update(ebooks);
                _context.SaveChanges();
                return Ok();
            }
            return BadRequest();

        }
        [Authorize]
        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteEBook(int id)
        {
            var ebook =  _context.Products.OfType<EBookModel>().FirstOrDefault(e => e.Id == id);
            if(ebook != null)
            {
                _context.Products.Remove(ebook);
                await _context.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
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
                var newEBooks = CSVHelper.FromFile<EBookModel>(file);

                if (newEBooks != null)
                {
                    var ebooks = await _context.Products.OfType<EBookModel>().ToListAsync();

                    foreach (var ebook in ebooks)
                    {
                        if (newEBooks.Any(x => x.Name == ebook.Name))
                        {
                            newEBooks.RemoveAll(x => x.Name == ebook.Name);
                        }
                    }

                    await _context.Products.AddRangeAsync(newEBooks);

                    await _context.SaveChangesAsync();
                }

                return Ok();
            }
        }
    }
}
