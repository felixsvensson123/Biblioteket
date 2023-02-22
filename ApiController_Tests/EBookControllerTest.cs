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
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Grupp15.Tests
{
    public class EBookControllerTest
    {
        private readonly AuthDbContext _context;
        private readonly EBooksController _controller;

        public EBookControllerTest()
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

            _context.Products.AddRange(new EBookModel()
            {
                Id = 1,
                Name = "Test Ebook",
                Description = "Desc Ebook"
            },
            new EBookModel()
            {
                Id = 2,
                Name = "Another Test Ebook",
                Description = "Test test"

            },
            new EBookModel()
            {
                Id=3,
                Name ="Ebook of test",
                Description = "Ebook of truth "
            });

            _context.SaveChanges();

            _controller = new(_context);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Role, "Librarian"), new Claim(ClaimTypes.Name, "Bodil@Mail.com") }));

            _controller.ControllerContext = new();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };
        }

        // *** Get all EBooks ***
        [Fact]
        public async Task Get_All_EBooks()
        {
            var ebooks = await _context.Products.OfType<EBookModel>().ToListAsync();

            var items = Assert.IsType<List<EBookModel>>(ebooks);
            Assert.Equal(3, items.Count);
        }

        // *** Get Ebook by Id ***
        [Fact]
        public async Task Get_EBook_By_Id()
        {
            var testId = 1;

            var ok = await _controller.GetEbook(testId);

            Assert.Equal("Test Ebook", ok.Name);
        }

        // *** Edit Ebook ***
        [Fact]
        public async Task Edit_Ebook()
        {
            var editedEBook = new EBookModel { Id = 1, Name = "New Name", Description = "New Desc" };

            _controller.EditEBook(editedEBook, 1);
            _context.SaveChanges();
            var newEbook = await _controller.GetEbook(editedEBook.Id);


            Assert.Equal(_context.Products.FirstOrDefault(x => x.Id == editedEBook.Id).Name, newEbook.Name);
            Assert.Equal(_context.Products.FirstOrDefault(x => x.Id == editedEBook.Id).Description, newEbook.Description);
        }

        // *** Delete Ebook ***
        [Fact]
        public async Task Delete_EBook()
        {
            var Ebook1 = _controller.GetEbook(1);
            var Ebook2 = _controller.GetEbook(2);
            var Ebook3 = _controller.GetEbook(3);

            await _controller.DeleteEBook(Ebook1.Id);
            await _controller.DeleteEBook(Ebook2.Id);
            await _controller.DeleteEBook(Ebook3.Id);

            Assert.Empty(_context.Products);
        }

        // *** Bulk Add ***
        [Fact]
        public async Task Bulk_Add()
        {
            using var stream = new MemoryStream();
            using var fileStream = new FileStream(@"C:\Users\Lukas\OneDrive\Documents\GitHub\Grupp15\EBooks.csv", FileMode.Open, FileAccess.Read); // Needs to be changed to local path when tested

            byte[] data = new byte[fileStream.Length];
            await fileStream.ReadAsync(data);
            await stream.WriteAsync(data);

            stream.Position = 0;

            FormFileCollection files = new();
            files.Add(new FormFile(stream, 0, fileStream.Length, "csv", "EBooks.csv"));
            _context.SaveChanges();
            FormCollection forms = new(null, files);

            _controller.Request.Form = forms;
            await _controller.BulkAdd();
            _context.SaveChanges();

            Assert.Equal(14, _context.Products.Count()); // 14 is expected since I already had 3 and added 11 more
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
