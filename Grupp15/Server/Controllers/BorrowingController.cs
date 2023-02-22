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
    public class BorrowingController : ControllerBase
    {
        private readonly AuthDbContext _context;

        public BorrowingController(AuthDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpPut("loan")]
        public async Task<IActionResult> LoanProduct([FromBody] int id)
        {
            var product = _context.Products.FirstOrDefault(x => x.Id == id);
            var user = await HttpContext.GetUserAsync(_context);

            if (user is not null && product is not null)
            {
                if (product.Count > 0)
                {
                    BorrowingModel borrow = new()
                    {
                        Product = product,
                        User = user,
                        BorrowingDate = DateTime.Now,
                        ReturnDate = DateTime.Now.Date.AddDays(7) //Placeholder date
                    };

                    product.Count--;

                    await _context.Borrowing.AddAsync(borrow);
                    await _context.SaveChangesAsync();

                    return Ok();
                }
                else if(product.ModelType == nameof(EBookModel))
                {
                    BorrowingModel borrow = new()
                    {
                        Product = product,
                        User = user,
                        BorrowingDate = DateTime.Now,
                        ReturnDate = DateTime.Now.Date.AddDays(7) //Placeholder date
                    };

                    await _context.Borrowing.AddAsync(borrow);
                    await _context.SaveChangesAsync();

                    return Ok();
                }
            }

            return BadRequest();
        }

        [Authorize]
        [HttpGet("loaned")]
        public async Task<IActionResult> GetLoaned()
        {
            var user = await HttpContext.GetUserAsync(_context);

            if (user is not null)
            {
                var userloaned = _context.Borrowing.Where(u => u.UserId == user.Id).Include(p => p.Product).ToList();

                if (userloaned is not null)
                    return Ok(userloaned);
            }

            return BadRequest();
        }

        [Authorize(Roles = "Admin, Librarian")]
        [HttpGet("loaned/{id}")]
        public async Task<IActionResult> GetLoanedById(string id)
        {
            var userloaned = _context.Borrowing.Where(u => u.UserId == id).Include(p => p.Product).ToList();

            if (userloaned is not null)
                return Ok(userloaned);

            return NotFound();
        }

        [Authorize]
        [HttpPut("return")]
        public async Task<IActionResult> ReturnProduct([FromBody] int productId)
        {
            var user = await HttpContext.GetUserAsync(_context);
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);

            if (user is not null && product is not null)
            {
                var borrow = await _context.Borrowing.FirstOrDefaultAsync(b => b.ProductId == productId);

                if (borrow is not null)
                {
                    product.Count++;

                    _context.Borrowing.Remove(borrow);

                    await _context.SaveChangesAsync();

                    return Ok();
                }

                return NotFound();
            }

            return BadRequest();
        }
    }
}