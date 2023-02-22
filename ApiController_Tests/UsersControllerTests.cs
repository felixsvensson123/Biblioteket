using Grupp15.Server.Controllers;
using Grupp15.Server.Data;
using Grupp15.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Grupp15.Tests
{
    public class UsersControllerTests
    {
        private readonly UsersController _controller;
        private readonly AuthDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        public UsersControllerTests()
        {
            var DbOptions = CreateNewContextOptions();
            _context = new AuthDbContext(DbOptions);
            var passHash = new PasswordHasher<ApplicationUser>();
           

            _context.Users.AddRange(new ApplicationUser()
            {
                Id = "9b177339-50a5-4dbf-a474-9a3a7bd94faf",
                Email = "Bodil@Mail.com",
                UserName = "Bodil@Mail.com",
                PersonName = "Bodil Bibliotekarie",
                Adress = "Malmö Stad",
                NormalizedEmail = "BODIL@MAIL.COM",
                NormalizedUserName = "BODIL@MAIL.COM",
                EmailConfirmed = true,
                PasswordHash = passHash.HashPassword(null!, "BodilAwesome12!")
            },
            new ApplicationUser()
            {
                Id = "9b177339-50a5-4dbf-a474-9a3a7bd94yuy",
                Email = "Pedro@Mail.com",
                UserName = "Pedro@Mail.com",
                PersonName = "Pedro Student",
                Adress = "Malmö Stad",
                NormalizedEmail = "PEDRO@MAIL.COM",
                NormalizedUserName = "PEDRO@MAIL.COM",
                EmailConfirmed = true,
                PasswordHash = passHash.HashPassword(null!, "PedroAwesome12!")
            });
            _context.SaveChanges();


            _controller = new(_userManager, _context, _configuration);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Role, "Librarian"), new Claim(ClaimTypes.Name, "Bodil@Mail.com") }));

            _controller.ControllerContext = new();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };
            
        }
        [Fact]
        public async void GetById_ExistingId_ReturnsOkResult()
        {
            var testId = "9b177339-50a5-4dbf-a474-9a3a7bd94faf";

            var okResult = await _controller.FindUser(testId);

            Assert.IsType<OkObjectResult>(okResult);
        }
        [Fact]
        public async void GetById_WrongId_ReturnsNotFound()
        {

            var noId = "";

            var notfound = await _controller.FindUser(noId);

            Assert.IsType<NotFoundResult>(notfound);
        }
        
        [Fact]
        public async void GetById_ExistingId_ReturnsRightUser()
        {
            var testId = "9b177339-50a5-4dbf-a474-9a3a7bd94yuy";

            var okResult = await _controller.FindUser(testId) as OkObjectResult;

            Assert.IsType<ApplicationUser>(okResult.Value);
            Assert.Equal(testId, (okResult.Value as ApplicationUser).Id);

        }

        private static DbContextOptions<AuthDbContext> CreateNewContextOptions()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var builder = new DbContextOptionsBuilder<AuthDbContext>();
            builder.UseInMemoryDatabase(databaseName: "LibraryDB")
                   .UseInternalServiceProvider(serviceProvider);

            return builder.Options;
        }
       
    }

    
  
}
