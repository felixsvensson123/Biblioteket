using Grupp15.Server.Data;
using Grupp15.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Grupp15.Server.Extensions;

namespace Grupp15.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthDbContext _context;
        private readonly IConfiguration _configuration;
        public UsersController(UserManager<ApplicationUser> userManager, AuthDbContext context, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterModel regUser)
        {
            var userExists = await _userManager.FindByNameAsync(regUser.Email);
            if (userExists != null)
                return BadRequest();

            ApplicationUser user = new()
            {
                Email = regUser.Email,
                UserName = regUser.Email,
                PersonName = regUser.PersonName,
                Adress = regUser.Adress
            };

            var result = await _userManager.CreateAsync(user, regUser.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Student");

                return Ok(user);
            }
            else
            { 
                return NoContent();
            }
        }
        [HttpPost("Login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginModel loginUser)
        {
            var user = await _userManager.FindByNameAsync(loginUser.Email);

            if (user != null && await _userManager.CheckPasswordAsync(user, loginUser.Password))
            {
                var roles = await _userManager.GetRolesAsync(user);

                List<Claim> authClaims = new()
                {
                    new Claim(ClaimTypes.Name, loginUser.Email)
                };

                foreach (var role in roles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }

                var token = GetToken(authClaims);
                var jwt = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(jwt);
            }

            return Unauthorized();
        }
        [Authorize]
        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteUser()
        {
            var user = await HttpContext.GetUserAsync(_context);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return Ok(result);
                }
                else
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return BadRequest();
        }
        [Authorize(Roles = LibraryRoles.Admin)]
        [HttpPut("HireLibrarian/{id}")]
        public async Task<IActionResult> HireLibrarian([FromBody] string id)
        {
            var librarian = await _context.Users.FirstAsync(l => l.Id == id);
            if(librarian != null)
            {
                await _userManager.AddToRoleAsync(librarian, "Librarian");
                await _userManager.RemoveFromRoleAsync(librarian, "Student");
                return Ok(librarian);
            }
            return BadRequest();
        }
        [Authorize(Roles = LibraryRoles.Admin)]
        [HttpPut("FireLibrarian/{id}")]
        public async Task<IActionResult> FireLibrarian([FromBody] string id)
        {
            var librarian = await _context.Users.FirstAsync(l => l.Id == id);
            if (librarian != null)
            {
                await _userManager.RemoveFromRoleAsync(librarian, "Librarian");
                await _userManager.AddToRoleAsync(librarian, "Student");
                return Ok(librarian);
            }
            return BadRequest();
        }
        [Authorize]
        [HttpGet("FindUser/{id}")]
        public async Task<IActionResult> FindUser(string id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(l => l.Id == id);
            if(user != null)
            {
                return Ok(user);
            }
            return NotFound();
        }
        [Authorize]
        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();

            return users != null ? Ok(users) : BadRequest();    
        }
        [Authorize]
        [HttpGet("GetCurrentUser")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = await HttpContext.GetUserAsync(_context);

            return user != null ? Ok(user) : NotFound();
        }
       
        [Authorize]
        [HttpGet("GetUserRole/{id}")]
        public async Task<IActionResult> GetUserRole(string id)
        {
            var user = await _context.Users.SingleAsync(u => u.Id == id);

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            var isLibrarian = await _userManager.IsInRoleAsync(user, "Librarian");

            var isStudent = await _userManager.IsInRoleAsync(user, "Student");
            if(isAdmin != null)
            {
                return Ok(isAdmin);
            } 
            else if(isLibrarian != null)
            {
                return Ok(isLibrarian);
            }
            else if(isStudent != null)
            {
                return Ok(isStudent);
            }
            return NotFound();  
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                expires: DateTime.Now.AddDays(45),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
        
    }

}
