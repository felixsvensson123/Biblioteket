using Grupp15.Client.Services;
using Grupp15.Server.Controllers;
using Grupp15.Server.Data;
using Grupp15.Shared.Helpers;
using Grupp15.Shared.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
namespace Grupp15.Tests
{
    public class BookControllerTests
    {
        public AuthDbContext context;
        [Fact]
        public async void TestGetBooks()
        {
            //Setup / Initialization
            var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
            context = new AuthDbContext(options);
            context.Database.EnsureCreated();

            context.Products.Add(new BookModel { Name = "JohnMc", Id = 1, Author = "Doby", Count = 2, Description = "Interesting stuff here" });
            context.Products.Add(new BookModel { Name = "JohnMc", Id = 3, Author = "Doby", Count = 2, Description = "Interesting stuff here" });
            context.Products.Add(new BookModel { Name = "JohnMc", Id = 4, Author = "Doby", Count = 2, Description = "Interesting stuff here" });

            var controller = new BooksController(context);
            var bookList = await controller.GetBooks();

            Assert.NotNull(bookList);
        }
        [Fact]
        public async void TestEditBook()
        {
            var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
            context = new AuthDbContext(options);
            context.Database.EnsureCreated();
            context.SaveChanges();
            var controller = new BooksController(context);

            var inputBook = context.Products.Add(new BookModel { Name = "OldJohn", Id = 1, Author = "Doby", Count = 2, Description = "Interesting stuff here" });
            context.SaveChanges();
            var editInput = (new BookModel { Name = "NewJohn", Id = 1, Author = "Doby", Count = 2, Description = "Interesting stuff here" });

            await controller.EditBook(editInput, 1);
            context.SaveChanges();
            var editedBook = controller.GetBook(editInput.Id);

            Assert.Same(context.Products.FirstOrDefault(x => x.Id == editedBook.Id).Name, editInput.Name);
        }
        [Fact]
        public async void TestDeleteBook()
        {
            var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
            context = new AuthDbContext(options);
            context.Database.EnsureCreated();
            context.SaveChanges();

            var controller = new BooksController(context);
            var movie = new BookModel { Name = "NewJohn", Id = 1, Author = "Doby", Count = 2, Description = "Interesting stuff here" };

            context.Products.Add(movie);
            context.SaveChanges();
            var movieToDelete = await controller.GetBook(1);
            await controller.DeleteBook(movieToDelete.Id);

            Assert.Empty(context.Products);
        }
        [Fact]
        public async void TestGetSingleBook()
        {
            var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
            context = new AuthDbContext(options);
            context.Database.EnsureCreated();
            context.SaveChanges();
            var controller = new BooksController(context);

            var movie = context.Products.Add(new BookModel { Name = "YogaLessons", Id = 1, Author = "John", Count = 2, Description = "Yo", Pages = 2 });
            context.SaveChanges();

            var thisBook = await controller.GetBook(1);

            Assert.Same("YogaLessons", thisBook.Name);
        }
        [Fact]
        public async Task TestAddBooks()
        {
            var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
            context = new AuthDbContext(options);
            context.Database.EnsureCreated();
            context.SaveChanges();

            var controller = new BooksController(context);
            controller.ControllerContext = new();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            using var stream = new MemoryStream();
            using var fileStream = new FileStream(@"C:\Users\Fakhro-Enetertainmen\Desktop\Skola\skola-repo-04-18\Grupp15V4\Grupp15\Books.csv", FileMode.Open, FileAccess.Read);

            byte[] data = new byte[fileStream.Length];
            await fileStream.ReadAsync(data);
            await stream.WriteAsync(data);

            stream.Position = 0;

            FormFileCollection files = new();
            files.Add(new FormFile(stream, 0, fileStream.Length, "csv", "Books.csv"));
            context.SaveChanges();
            FormCollection forms = new(null, files);

            //byt till din controller
            controller.Request.Form = forms;
            await controller.BulkAdd();
            context.SaveChanges();

            Assert.NotNull(context.Products);
            Assert.Equal(10, context.Products.Count());
        }
    }
}

