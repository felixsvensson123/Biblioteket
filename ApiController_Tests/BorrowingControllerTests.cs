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
    public class BorrowingControllerTests
    {
        private readonly BorrowingController _controller;
        private readonly AuthDbContext _context;

        public BorrowingControllerTests()
        {
            var DbOptions = CreateNewContextOptions();
            _context = new AuthDbContext(DbOptions);

            var passHash = new PasswordHasher<ApplicationUser>();

            _context.Users.Add(new ApplicationUser()
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
            });

            _context.Products.AddRange(new MovieModel()
            {
                Id = 1,
                Name = "Kalle Anka",
                Description = "Testa",
                Count = 4,
            },
            new MovieModel()
            {
              Id = 2,
              Name = "Test",
              Description = "Testare",
              Count  = 1, 
            });
            _context.Products.AddRange(new BookModel()
            {
                Id = 3,
                Name = "No Name Book",
                Description = "Wat a",
                Count = 1,
            },
            new BookModel()
            {
                Id = 4,
                Name = "Testarens Bok",
                Description = "Hmm",
                Count = 0,
            });
            _context.Products.AddRange(new EBookModel()
            {
                Id = 5,
                Name = "No Name Ebook",
                Description = "grupp",
            },
            new EBookModel()
            {
                Id = 6,
                Name = "Testarens Ebok",
                Description = "Pallar inte jobba"
            });

            _context.SaveChanges();
            _controller = new(_context);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Role, "Librarian"), new Claim(ClaimTypes.Name, "Bodil@Mail.com") }));

            _controller.ControllerContext = new();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };
        }

        // *** Borrow Product Tests ***
        [Fact]
        public async void Test_BorrowProduct()
        {
            var Book = 3;

            var borrow = await _controller.LoanProduct(Book);

            Assert.IsType<OkResult>(borrow);
        }
        [Fact]
        public async void Test_Borrow_Book_Twice_Only_One_InStock()
        {
            var book = 3;

            var borrow = await _controller.LoanProduct(book);

            Assert.IsType<OkResult>(borrow);

            var borrow2 = await _controller.LoanProduct(book);

            Assert.IsType<BadRequestResult>(borrow2);
        }
        [Fact]
        public async void Borrowing_Returns_BadRequest()
        {
            var book = 10;

            var borrow = await _controller.LoanProduct(book);   

            Assert.IsType<BadRequestResult>(borrow);
        }
        [Fact]
        public async void Attempt_To_Borrow_A_Product_With_0_In_Count()
        {
            var id = 4;

            var borrow = await _controller.LoanProduct(id);

            Assert.IsType<BadRequestResult>(borrow);
        }

        // *** Get Loaned Products Tests ***
        [Fact]
        public async void Get_Correct_UserId_From_Borrowed_Book()
        {
            var book = 3;
            var userId = "9b177339-50a5-4dbf-a474-9a3a7bd94faf";
            var borrow = await _controller.LoanProduct(book);
            Assert.IsType<OkResult>(borrow);
            
            var getloaned = await _controller.GetLoanedById(userId);

            Assert.IsType<OkObjectResult>(getloaned);
        }
        [Fact]
        public async void Get_All_Products_Borrowed_By_This_Person()
        {
            var book = 1;
            var book2 = 2;

            var userId = "9b177339-50a5-4dbf-a474-9a3a7bd94faf";

            await _controller.LoanProduct(book);
            await _controller.LoanProduct(book2);

            var GetAllLoaned = await _controller.GetLoanedById(userId) as OkObjectResult;

            var loaned = Assert.IsType<List<BorrowingModel>>(GetAllLoaned.Value);
            Assert.Equal(2, loaned.Count);
        }

        // *** Return Product Tests *** 
        [Fact]
        public async void Return_Borrowed_Book()
        {
            var book = 1; 

            var borrow = await _controller.LoanProduct(book);

            Assert.IsType<OkResult>(borrow);
            var returnbook = await _controller.ReturnProduct(book);

            Assert.IsType<OkResult>(returnbook);
        }
        [Fact]
        public async void Return_A_Book_That_User_Hasent_Borrowed_Returns_NotFoundResult()
        {
            var book = 1;

            var returnbook = await _controller.ReturnProduct(book);

            Assert.IsType<NotFoundResult>(returnbook);
        }
        [Fact]
        public async void Return_A_Book_From_Another_Library_Returns_BadRequest()
        {
            var book = 10;

            var returnbook = await _controller.ReturnProduct(book);
            Assert.IsType<BadRequestResult>(returnbook);
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
