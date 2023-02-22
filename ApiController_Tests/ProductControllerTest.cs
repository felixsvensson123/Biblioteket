using Grupp15.Server.Controllers;
using Grupp15.Server.Data;
using Grupp15.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Grupp15.Tests
{
    public class ProductControllerTest
    {

        private readonly AuthDbContext _context;
        private readonly ProductController _controller;

        public ProductControllerTest()
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
           
            _context.Products.AddRange(new MovieModel(){
                Id = 1, Name ="Kalle Anka", Description = "Testa"
            },
            new MovieModel() { 
                Id = 2, Name = "Test", Description = "Testare"
            });
            _context.Products.AddRange(new BookModel()
            {
                Id = 3,
                Name = "No Name Book",
                Description = "Wat a"
            },
            new BookModel()
            {
                Id = 4, 
                Name = "Testarens Bok",
                Description = "Hmm",
            });
            _context.Products.AddRange(new EBookModel()
            {
                Id = 5, Name = "No Name Ebook",
                Description = "grupp"
            },
            new BookModel()
            {
                Id = 6, Name = "Testarens Ebok",
                Description = "Pallar inte jobba"
            });

            _context.SaveChanges();

            _controller = new(_context);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Role, "Librarian"), new Claim(ClaimTypes.Name, "Bodil@Mail.com") }));

            _controller.ControllerContext = new();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };
        }

        // *** GetAllProducts Tests *** 
        [Fact]
        public async Task Get_All_Products()
        {
            var products = await _context.Products.ToListAsync();

            var items = Assert.IsType<List<ProductBase>>(products);
            Assert.Equal(6, items.Count);
        }

        // *** GetById Tests *** 
        [Fact]
        public async Task GetProduct_ById_Assert_Is_BookModel()
        {
            var testId = 3;

            var ok = await _controller.GetProduct(testId) as OkObjectResult;

            Assert.IsType<BookModel>(ok.Value);
            Assert.Equal(testId, (ok.Value as BookModel).Id);
        }
        [Fact]
        public async Task GetProduct_ById_Assert_Is_MovieModel()
        {
            var testId = 2;

            var ok = await _controller.GetProduct(testId) as OkObjectResult;

            Assert.IsType<MovieModel>(ok.Value);
            Assert.Equal(testId, (ok.Value as MovieModel).Id);
        }
        [Fact]
        public async Task Put_Wrong_Id_In_GetProduct()
        {
            var wrongid = 15;

            var ok = await _controller.GetProduct(wrongid);

            Assert.IsType<BadRequestResult>(ok);
        }
        [Fact]
        public async Task Search_For_Products()
        {
            var search = "Kalle Anka";

            var ok = await _controller.SearchProducts(search);

            Assert.IsType<OkObjectResult>(ok.Result);
        }
        [Fact]
        public async Task Search_For_Wrong_Product_Returns_Null()
        {
            var search = "This wonderfull book doesnt exist!";

            var ok = await _controller.SearchProducts(search);

            Assert.Null(ok.Value);
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
