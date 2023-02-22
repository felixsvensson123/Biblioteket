using Microsoft.AspNetCore.Identity;

namespace Grupp15.Shared.Models;

public class ApplicationUser : IdentityUser
{
    public string? PersonName { get; set; }
    public string? Adress { get; set; }
}