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
using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;
using Assert = Xunit.Assert;
namespace Grupp15.Tests
{
    public class MoviesControllerTests
    {
        public AuthDbContext context { get; private set; }
        [Fact]
        public void TestGetMovies()
        {
            //Setup / Initialization
            var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
            var context = new AuthDbContext(options);
            context.Database.EnsureCreated();
            context.SaveChanges();
            //input
            context.Products.Add(new MovieModel { Name = "123", Count = 2, Description = "Something interesting", Id = 1, Genre = "Interesting", Director = "Steven Mcqueen" }); ;
            context.Products.Add(new MovieModel { Name = "321", Count = 2, Description = "Something interesting", Id = 2, Genre = "Interesting", Director = "Steven Mcqueen" }); ;
            context.Products.Add(new MovieModel { Name = "456", Count = 2, Description = "Something interesting", Id = 3, Genre = "Interesting", Director = "Steven Mcqueen" }); ;
            context.SaveChanges();

            var controller = new MoviesController(context);

            //output
            var movieList = controller.GetMovieList();
            //jämför input med output
            Assert.NotNull(movieList);
            //Assert.Cou(3, m);
        }
        [Fact]
        public async void TestEditMovie()
        {
            var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
            context = new AuthDbContext(options);
            context.Database.EnsureCreated();
            context.SaveChanges();
            var controller = new MoviesController(context);

            var inputMovie = context.Products.Add(new MovieModel { Name = "OldName", Id = 1, Count = 2, Description = "Old", Director = "OldJohn", Genre = "OldOG" });
            context.SaveChanges();
            var editInput = new MovieModel { Name = "NewName", Id = 1, Count = 4, Description = "New", Director = "NewJohn", Genre = "NewOG" };

            await controller.EditBook(editInput, 1);
            context.SaveChanges();
            var editedMovie = controller.GetMovie(editInput.Id);

            Assert.Same(context.Products.FirstOrDefault(x => x.Id == editedMovie.Id).Name, editInput.Name);
        }
        [Fact]
        public async void TestDeleteMovie()
        {
            var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
            context = new AuthDbContext(options);
            context.Database.EnsureCreated();
            context.SaveChanges();

            var controller = new MoviesController(context);
            var movie = new MovieModel { Name = "OldName", Id = 1, Count = 2, Description = "Old", Director = "OldJohn", Genre = "OldOG" };

            context.Products.Add(movie);
            context.SaveChanges();
            var movieToDelete = await controller.GetMovie(1);
            await controller.DeleteMovie(movieToDelete.Id);

            Assert.Empty(context.Products);
        }
        [Fact]
        public async void TestGetSingleMovie()
        {
            var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
            context = new AuthDbContext(options);
            context.Database.EnsureCreated();
            context.SaveChanges();
            context.Products.Add(new MovieModel { Name = "YogaLessons", Id = 1, Count = 2, Description = "Interesting", Genre = "Action", Director = "Ghislaine" });
            context.SaveChanges();

            var controller = new MoviesController(context);

            var inputId = 1;
            var inputMovie = await controller.GetMovie(inputId);

            Assert.Same("YogaLessons", inputMovie.Name);
        }

        [Fact]
        public async Task TestAddMovies()
        {
            var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
            context = new AuthDbContext(options);
            context.Database.EnsureCreated();
            context.SaveChanges();

            var controller = new MoviesController(context);
            controller.ControllerContext = new();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            using var stream = new MemoryStream();
            using var fileStream = new FileStream(@"C:\Users\Fakhro-Enetertainmen\Desktop\Skola\skola-repo-04-18\Grupp15V4\Grupp15\Movies.csv", FileMode.Open, FileAccess.Read);

            byte[] data = new byte[fileStream.Length];
            await fileStream.ReadAsync(data);
            await stream.WriteAsync(data);

            stream.Position = 0;

            FormFileCollection files = new();
            files.Add(new FormFile(stream, 0, fileStream.Length, "csv", "Movies.csv"));
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