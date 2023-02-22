using Grupp15.Server.Data;
using Grupp15.Server.Extensions;
using Grupp15.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Grupp15.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly AuthDbContext _context;

        public NewsController(AuthDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpPost("News")]
        public async Task<IActionResult> AddNews([FromBody] string news)
        {
            var user = await HttpContext.GetUserAsync(_context);

            if (news == null)
            {
                return BadRequest();
            }
            else if (news != null)
            {
                NewsModel content = new NewsModel
                {
                    News = news,
                    User = user,
                    Created = DateTime.Now,

                };

                _context.News.Add(content);

                await _context.SaveChangesAsync();

                return Ok();

            }

            return NotFound();

        }
        [HttpGet("GetNews")]
        public async Task<IActionResult> GetMessage()
        {
            var news = await _context.News.ToListAsync();
            foreach (var i in news)
            {
                i.User = (await _context.Users.ToListAsync()).Find(u => u.Id == i.UserId);
            }

            List<NewsModel> sortNews = news.OrderByDescending(i => i.Created).ToList();

            if (sortNews != null)
                return Ok(sortNews);
            else
                return NotFound();
        }
        [Authorize]
        [HttpGet("GetNews/{id}")]
        //Get specific news, used for viewing when editing
        public async Task<IActionResult> GetEditNews(int id)
        {
            var news = (await _context.News.ToListAsync()).Find(u => u.Id == id);
           
            if (news != null)
                return Ok(news);
            else return NotFound();
        }
        [Authorize]
        [HttpDelete("NewsDelete/{Id}")]
        public async Task<IActionResult> DeleteNews(int id)
        {
            var news = await _context.News.FirstOrDefaultAsync(n => n.Id == id);
            if (news == null)
            {
                return NotFound();
            }
            else
            {
                _context.News.Remove(news);
                await _context.SaveChangesAsync();
                return Ok();
            }
        }
        [Authorize]
        [HttpPut("EditNews/{id}")]
        public async Task<IActionResult> UpdateNews([FromBody] string news, int id)
        {
            var selectedNews = await _context.News.FirstOrDefaultAsync(n => n.Id == id);
            var user = await HttpContext.GetUserAsync(_context);
            if (selectedNews == null)
            {
                return BadRequest();
            }
            else
            {
                selectedNews.News = news;
                await _context.SaveChangesAsync();
                return Ok();
            }
        }
    }

}
