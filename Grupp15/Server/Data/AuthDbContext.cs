using Grupp15.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Grupp15.Server.Data
{
    public class AuthDbContext : IdentityDbContext<ApplicationUser>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options)
            : base(options)
        {
        }

        public DbSet<ProductBase> Products { get; set; }
        public DbSet<BorrowingModel> Borrowing { get; set; }
        public DbSet<NewsModel> News { get; set; }
        public DbSet<LectureModel> Lectures { get; set; }
        public DbSet<AttendModel> Attending { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var hash = new PasswordHasher<ApplicationUser>();
            var adminRole = new IdentityRole() { Name = "Admin", NormalizedName = "ADMIN", Id = "d7fc4ba6-4957-41a7-96b5-52b65c06bc35" };
            var librarianRole = new IdentityRole() { Name = "Librarian", NormalizedName = "LIBRARIAN", Id = "ef57ac2e-9af4-4bdb-a8e4-a54097032e04" };
            var studentRole = new IdentityRole() { Name = "Student", NormalizedName = "STUDENT", Id = "d153c726-e709-4946-824b-0ed63bbf136a" };
            var storeOwner = new ApplicationUser()
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
            };

            modelBuilder.Entity<IdentityRole>().HasData(adminRole, librarianRole, studentRole);

            modelBuilder.Entity<ApplicationUser>().HasData(storeOwner);

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>()
            {
                RoleId = adminRole.Id,
                UserId = storeOwner.Id,
            });

            modelBuilder.Entity<NewsModel>().HasData(new NewsModel()
            {
                Id = 1,
                News = "Hej alla! :) Bodil här :^) Jag tänkte bara höra om någon har en bättre idé för hur nyheterna ska presenteras? :O Trevlig vistelse på mitt bibliotek!",
                UserId = storeOwner.Id,
                Created = DateTime.Now,

            }); 
            modelBuilder.Entity<LectureModel>().HasData(new LectureModel()
            {
                Id =1,
                Subject = "Database",
                Description = "How to Learn Databases",
                LecturerName = "Jason",
                EstimatedLengthMins = 480,
                StartDate = DateTime.Now,
                MaxStudentCount = 20,
                AttendingCount = 0,

            });

            modelBuilder.Entity<ProductBase>()
                .HasDiscriminator<string>("Discriminator")
                .HasValue<BookModel>("Book")
                .HasValue<MovieModel>("Movie")
                .HasValue<EBookModel>("Ebook");
        }
    }
}
