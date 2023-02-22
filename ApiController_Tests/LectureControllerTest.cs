using Grupp15.Server.Controllers;
using Grupp15.Server.Data;
using Grupp15.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Grupp15.Tests
{
    public class LectureControllerTest
    {
        private readonly AuthDbContext _context;

        private readonly LectureController _lectureController;

        public LectureControllerTest()
        {
            var dbContextOptions = CreateNewContextOptions();

            _context = new AuthDbContext(dbContextOptions);

            _context.Lectures.Add(new LectureModel()
            {
                LecturerName = "aaaaa",
                EstimatedLengthMins = 123,
                Subject = "databases!!",
                Description = "bbbb",
                MaxStudentCount = 10,
                StartDate = DateTime.Now
            });

            var hash = new PasswordHasher<ApplicationUser>();

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
                PasswordHash = hash.HashPassword(null!, "BodilAwesome12!")
            });

            _context.SaveChanges();
            _lectureController = new LectureController(_context);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Role, LibraryRoles.Librarian), new Claim(ClaimTypes.Name, "Bodil@Mail.com") }));

            _lectureController.ControllerContext = new();
            _lectureController.ControllerContext.HttpContext = new DefaultHttpContext { User = user };
        }

        [Fact]
        public async Task Signup_To_Valid_Lecture_Should_Return_Ok()
        {
            var response = await _lectureController.SignupForLecture(1) as StatusCodeResult;

            Assert.IsType<OkResult>(response);
        }

        [Fact]
        public async Task Signup_To_Invalid_Lecture_Should_Return_NotFound()
        {
            var response = await _lectureController.SignupForLecture(999) as StatusCodeResult;

            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task Signup_To_Lecture_Twice_Should_Return_BadRequest()
        {
            var okResponse = await _lectureController.SignupForLecture(1) as StatusCodeResult;

            var badRequestResponse = await _lectureController.SignupForLecture(1) as StatusCodeResult;

            Assert.IsType<OkResult>(okResponse);

            Assert.IsType<BadRequestResult>(badRequestResponse);
        }

        [Fact]
        public async Task Get_Lecture_By_Valid_Id_Should_Return_Model()
        {
            var response = await _lectureController.GetById(1) as ObjectResult;

            Assert.IsType<LectureModel>(response.Value);
        }

        [Fact]
        public async Task Get_Lecture_By_Invalid_Id_Should_Return_NotFound()
        {
            var response = await _lectureController.GetById(99) as StatusCodeResult;

            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task Get_All_Lectures_Should_Return_List()
        {
            var response = await _lectureController.GetAll() as ObjectResult;

            Assert.IsType<List<LectureModel>>(response.Value);
        }

        [Fact]
        public async Task Add_Valid_Lecture_Should_Return_Ok()
        {
            var lecture = new LectureModel()
            {
                LecturerName = "test",
                EstimatedLengthMins = 120,
                Description = "unit tests",
                MaxStudentCount = 10,
                Subject = "tests",
                StartDate = DateTime.Now,
            };

            var response = await _lectureController.AddLecture(lecture) as StatusCodeResult;

            Assert.IsType<OkResult>(response);
        }

        [Fact]
        public async Task Add_Invalid_Lecture_Should_Return_BadRequest()
        {
            var lecture = new LectureModel();

            var response = await _lectureController.AddLecture(lecture) as StatusCodeResult;

            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task Edit_Lecture_Should_Return_Ok_And_Update_In_DB()
        {
            var response = (ObjectResult)await _lectureController.GetById(1);

            var lecture = (LectureModel)response.Value;

            lecture.Subject = "testing";

            var editResponse = (StatusCodeResult)await _lectureController.EditLecture(lecture, 1);

            Assert.IsType<OkResult>(editResponse);

            var newLecture = await _context.Lectures.FirstOrDefaultAsync(x => x.Subject == "testing");

            Assert.Equal("testing", newLecture.Subject);
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