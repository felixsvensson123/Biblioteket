using Grupp15.Server.Data;
using Grupp15.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Grupp15.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly AuthDbContext _context;

        public ProductController(AuthDbContext context)
        {
            _context = context;
        }

        [HttpGet("Prods")]
        public async Task<IActionResult> GetLatestsProducts()
        {
            var prod = await _context.Products.ToListAsync();

            List<ProductBase> sortProd = prod.OrderByDescending(i => i.Created).Take(15).ToList();

            if (sortProd != null)
                return Ok(sortProd);
            else
                return NotFound();
        }
        [HttpGet("AllProds")]
        public async Task<IActionResult> GetAllProducts()
        {
            var prod = await _context.Products.ToListAsync();
            List<ProductBase> sortProd = prod.OrderByDescending(i => i.Name).ToList();
            if (sortProd != null)
                return Ok(sortProd);
            else return NotFound();
        }
        [HttpGet("GetLoaned")]
        public async Task<IActionResult> GetLoaned()
        {
            var userloaned =  _context.Borrowing.Include(x => x.User).Include(v => v.Product).ToList();
            List<BorrowingModel> sortLoaned = userloaned.OrderByDescending(i => i.Product?.Name).ToList();
            if (sortLoaned != null)
                return Ok(sortLoaned);
            else
                return NotFound();
        }
        [HttpGet("GetProduct/{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var prod = (await _context.Products.ToListAsync()).Find(u => u.Id == id);
            return prod != null ? Ok(prod) : BadRequest();    
        }

        [HttpGet("Search/{searchText}")]
        public async Task<ActionResult<List<ProductBase>>> SearchProducts(string searchText)
        {
            var prod = (await _context.Products
                .Where(p => p.Name.Contains(searchText) || p.Description.Contains(searchText))
                .ToListAsync());
            return Ok(prod);
        }
    }
}
