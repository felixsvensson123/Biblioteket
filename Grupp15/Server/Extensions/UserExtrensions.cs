using Grupp15.Server.Data;
using Grupp15.Shared.Models;
using Microsoft.EntityFrameworkCore;
namespace Grupp15.Server.Extensions
{
    internal static class UserExtensions
    {
        public static async Task<ApplicationUser?> GetUserAsync(this HttpContext context, AuthDbContext authContext)
        {
            return await authContext.Users.SingleAsync(u => u.Email.ToLower() == context.User.Identity!.Name!.ToLower());
        }
    }
}
