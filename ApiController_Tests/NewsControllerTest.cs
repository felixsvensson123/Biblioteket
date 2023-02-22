using Grupp15.Server.Controllers;
using Grupp15.Server.Data;
using Grupp15.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using Xunit;

namespace Grupp15.Tests
{
    public class NewsControllerTest
    {
        private readonly NewsController _controller;
        private readonly AuthDbContext DbContext;

        public NewsControllerTest()
        {
            //modelBuilder.Entity<ApplicationUser>().HasData(storeOwner);
            var DbOptions = CreateNewContextOptions();
            DbContext = new AuthDbContext(DbOptions);

            var passHash = new PasswordHasher<ApplicationUser>();

            DbContext.Users.Add(new ApplicationUser()
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



            DbContext.News.AddRange(
                new NewsModel(){ Id = 1, News = "Hej Hej Bodil Här", UserId = "9b177339-50a5-4dbf-a474-9a3a7bd94faf" }, 
                new NewsModel(){ Id = 2, News = "Test test", UserId = "9b177339-50a5-4dbf-a474-9a3a7bd94faf" }, 
                new NewsModel(){ Id = 3, News = "Wat a test", UserId = "9b177339-50a5-4dbf-a474-9a3a7bd94faf" }
            );

            DbContext.SaveChanges();
            _controller = new(DbContext);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Role, "Librarian"), new Claim(ClaimTypes.Name, "Bodil@Mail.com") }));

            _controller.ControllerContext = new();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };
        }
        // *** GetAllNews Tests *** 
        [Fact]
        public async void Get_News_ShouldReturnNotNull()
        {
            var ok = await _controller.GetMessage() as OkObjectResult;

            var news = Assert.IsType<List<NewsModel>>(ok.Value);
            Assert.Equal(3, news.Count);

        }
        // *** GetEditNews by Id Tests ***
        [Fact]
        public async void Get_EditNews_By_Id()
        {
            var testid = 1;
            var ok = await _controller.GetEditNews(testid) as OkObjectResult;

            Assert.IsType<NewsModel>(ok.Value);
            Assert.Equal(testid, (ok.Value as NewsModel).Id);

        }
        [Fact]
        public async void Pass_Wrong_Id_Returns_NotFound()
        {
            var testid = 0;
            var notfound = await _controller.GetEditNews(testid);

            Assert.IsType<NotFoundResult>(notfound);

        }
        // *** AddNews Tests *** 
        [Fact]
        public async void Add_New_News_To_DB()
        {

            var news = new NewsModel()
            {
                News = "Testing the news, Helloooooo",
                UserId = "9b177339-50a5-4dbf-a474-9a3a7bd94faf",

            };

            var createNews = await _controller.AddNews(news.News);
            Assert.IsType<OkResult>(createNews);
        }

        // *** DeleteNews Tests *** 
        [Fact]
        public async void Remove_News_With_WrongId()
        {
            var newsId = 4;

            var wrongid = await _controller.DeleteNews(newsId);
            Assert.IsType<NotFoundResult>(wrongid);
        }
        [Fact]
        public async void Remove_News_With_Id()
        {
            var newsid = 1;
            var Del = await _controller.DeleteNews(newsid);

            Assert.IsType<OkResult>(Del);
        }
        [Fact]
        public async void Remove_News_With_Id_And_Ensure_News_Is_Deleted()
        {
            var newsId = 1;

            var ok = await _controller.DeleteNews(newsId);

            ok = await _controller.GetEditNews(newsId);

            Assert.IsType<NotFoundResult>(ok);
        }
        [Fact]
        public async void Remove_News_With_Id_Expect_List_Returns_2()
        {
            var newsId = 2;

            await _controller.DeleteNews(newsId);

            var list = await _controller.GetMessage() as OkObjectResult;

            var news = Assert.IsType<List<NewsModel>>(list.Value);
            Assert.Equal(2, news.Count);

        }

        // *** UpdateNews Tests *** 
        [Fact]
        public async void Update_Selected_News()
        {
            var newsId = 1;

            var NewsUpdate = "Hello test test whaaat";
            var update = await _controller.UpdateNews(NewsUpdate, newsId);

            Assert.IsType<OkResult>(update);
        }
        [Fact]
        public async void Update_Selected_News_WrongId()
        {
            var wrongid = 4;
            var NewsUpdate = "Hello test test whaat";

            var update = await _controller.UpdateNews(NewsUpdate, wrongid);

            Assert.IsType<BadRequestResult>(update);

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
