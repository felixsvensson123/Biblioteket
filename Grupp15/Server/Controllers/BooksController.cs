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
    public class BooksController : ControllerBase
    {
        private readonly AuthDbContext _context;

        public BooksController(AuthDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<BookModel>> GetBooks()
        {
            return await GetFullyIncludedBooks();
        }

        async Task<IEnumerable<BookModel>> GetFullyIncludedBooks()
        {
            List<BookModel> dbBooks = await _context.Products.OfType<BookModel>().ToListAsync();

            return dbBooks;
        }

        [HttpGet("{id}")]
        public async Task<BookModel> GetBook(int id)
        {
            var result = await GetFullyIncludedBooks();

            return result.SingleOrDefault(b => b.Id == id);
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
                var newBooks = CSVHelper.FromFile<BookModel>(file);

                if (newBooks != null)
                {
                    var books = await _context.Products.OfType<BookModel>().ToListAsync();

                    foreach (var book in books)
                    {
                        if (newBooks.Any(x => x.Name == book.Name))
                        {
                            newBooks.RemoveAll(x => x.Name == book.Name);
                        }
                    }

                    await _context.Products.AddRangeAsync(newBooks);

                    await _context.SaveChangesAsync();
                }

                return Ok();
            }
        }

        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> EditBook([FromBody] BookModel book, int id)
        {
            var books = await _context.Products.OfType<BookModel>().FirstOrDefaultAsync(x => x.Id == id);
            if (books != null)
            {
                books.Author = book.Author;
                books.Pages = book.Pages;
                books.Count = book.Count;
                books.Name = book.Name;
                books.Description = book.Description;

                _context.Update(books);
                _context.SaveChanges();
                return Ok();
            }
            return BadRequest();

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Products.OfType<BookModel>().FirstOrDefaultAsync(i => i.Id == id);
            if (book != null)
            {
                _context.Products.Remove(book);
                _context.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }
    }
}