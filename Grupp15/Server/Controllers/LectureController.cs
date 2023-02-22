using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Grupp15.Server.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Grupp15.Shared.Models;
using Grupp15.Server.Extensions;

namespace Grupp15.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LectureController : Controller
    {
        private readonly AuthDbContext _context;

        public LectureController(AuthDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var lectures = await _context.Lectures.ToListAsync();

            return Ok(lectures);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var lecture = await _context.Lectures.FirstOrDefaultAsync(x => x.Id == id);

            if (lecture != null)
                return Ok(lecture);

            return NotFound();
        }

        [Authorize]
        [HttpPost("Signup")]
        public async Task<IActionResult> SignupForLecture([FromBody] int id)
        {
            var user = await HttpContext.GetUserAsync(_context);

            var lecture = await _context.Lectures.FirstOrDefaultAsync(x => x.Id == id);

            if (lecture is not null)
            {
                if (lecture.AttendingCount + 1 <= lecture.MaxStudentCount)
                {
                    var attending = _context.Attending.Where(x => x.UserId == user!.Id).ToList();

                    bool isSignedUp = attending.Find(x => x.LectureId == id) != null;

                    if (!isSignedUp)
                    {
                        lecture.AttendingCount++;

                        await _context.Attending.AddAsync(new AttendModel() { Lecture = lecture, User = user });

                        await _context.SaveChangesAsync();

                        return Ok();
                    }

                    return BadRequest();
                }
            }

            return NotFound();
        }

        [Authorize]
        [HttpPost("Unregister")]
        public async Task<IActionResult> UnregisterFromLecture([FromBody] int id)
        {
            var user = await HttpContext.GetUserAsync(_context);

            var lecture = await _context.Lectures.FirstOrDefaultAsync(x => x.Id == id);

            if (lecture is not null && user is not null)
            {
                var attending = _context.Attending.Where(x => x.UserId == user.Id).ToList();

                bool isSignedUp = attending.Find(x => x.LectureId == id) != null;

                if (isSignedUp)
                {
                    lecture.AttendingCount--;

                    _context.Attending.Remove(attending.Find(x => x.LectureId == id)!);

                    await _context.SaveChangesAsync();

                    return Ok();
                }

                return BadRequest();
            }

            return NotFound();
        }

        [Authorize(Roles = "Admin, Librarian")]
        [HttpPost("add")]
        public async Task<IActionResult> AddLecture([FromBody] LectureModel newLecture)
        {
            if (newLecture != null)
            {
                if (newLecture.LecturerName != string.Empty || newLecture.Description != string.Empty || newLecture.Subject != string.Empty)
                {
                    await _context.Lectures.AddAsync(newLecture);
                    await _context.SaveChangesAsync();

                    return Ok();
                }
            }

            return BadRequest();
        }

        [Authorize(Roles = "Admin, Librarian")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLecture(int id)
        {
            var lecture = await _context.Lectures.FirstOrDefaultAsync(x => x.Id == id);

            if (lecture != null)
            {
                _context.Lectures.Remove(lecture);

                await _context.SaveChangesAsync();

                return Ok();
            }

            return NotFound();
        }

        [Authorize(Roles = "Admin, Librarian")]
        [HttpPut("{id}")]
        public async Task<IActionResult> EditLecture([FromBody] LectureModel edited, int id)
        {
            var lecture = await _context.Lectures.FirstOrDefaultAsync(x => x.Id == id);

            if (lecture is not null)
            {
                lecture.Subject = edited.Subject;
                lecture.Description = edited.Description;
                lecture.StartDate = edited.StartDate;
                lecture.LecturerName = edited.LecturerName;
                lecture.EstimatedLengthMins = edited.EstimatedLengthMins;
                lecture.MaxStudentCount = edited.MaxStudentCount;

                await _context.SaveChangesAsync();

                return Ok();
            }

            return NotFound();
        }

        [Authorize(Roles = "Admin, Librarian")]
        [HttpGet("Attending/user/{id}")]
        public async Task<IActionResult> GetAttendingByUserId(string id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user is not null)
            {
                var attending = _context.Attending.Where(a => a.UserId == user.Id).Include(a => a.Lecture).Include(a => a.User).ToList();

                return Ok(attending);
            }

            return BadRequest();
        }

        [Authorize(Roles = "Admin, Librarian")]
        [HttpGet("Attending/lecture/{id}")]
        public async Task<IActionResult> GetAttendingByLectureId(int id)
        {
            var attending = _context.Attending.Where(x => x.LectureId == id).Include(x => x.Lecture).Include(x => x.User).ToList();

            return Ok(attending);
        }

        [Authorize]
        [HttpGet("Attending")]
        public async Task<IActionResult> GetAttending()
        {
            var user = await HttpContext.GetUserAsync(_context);

            if (user is not null)
            {
                var attending = _context.Attending.Where(a => a.UserId == user.Id).Include(a => a.Lecture).Include(a => a.User).ToList();

                return Ok(attending);
            }

            return BadRequest();
        }

        [Authorize(Roles = "Admin, Librarian")]
        [HttpPut("RemoveStudent/{lectureId}")]
        public async Task<IActionResult> RemoveStudent(int lectureId, [FromBody] string studentId)
        {
            var student = await _context.Users.FirstOrDefaultAsync(u => u.Id == studentId);

            var lecture = await _context.Lectures.FirstOrDefaultAsync(x => x.Id == lectureId);

            if (student is not null && lecture is not null)
            {
                var attend = await _context.Attending.FirstOrDefaultAsync(x => x.LectureId == lectureId);

                if(attend is not null)
                {
                    lecture.AttendingCount--;
                    _context.Attending.Remove(attend);

                    await _context.SaveChangesAsync();
                    return Ok();
                }

                return BadRequest();
            }

            return NotFound();
        }
    }
}