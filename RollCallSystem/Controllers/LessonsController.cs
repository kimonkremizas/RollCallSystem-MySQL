#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RollCallSystem.APIModels;
using RollCallSystem.Database;
using RollCallSystem.Logic;

namespace RollCallSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LessonsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly TrimmedUser _mockUser = default;

        public LessonsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public LessonsController(ApplicationDbContext context, TrimmedUser mockUser)
        {
            _context = context;
            _mockUser = mockUser;
        }

        // GET ALL LESSONS OF USER BY MONTH: api/Lessons/ByMonth/5
        [HttpGet("ByMonth/{monthNo}")]
        [Authorize(Roles = "Teacher, Student")]
        public async Task<ActionResult<List<TrimmedLesson>>> GetByMonth(int monthNo)
        {
            string userId;
            string userRole;

            if(_mockUser == default)
            {
                userId = User.FindFirst(ClaimTypes.Name)?.Value;
                userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            }
            else
            {
                userId = _mockUser.Id.ToString();
                userRole = HelperFunctions.GetRoleName(_mockUser.RoleId);
            }

            List<Lesson> lessons = new List<Lesson>();

            if(userRole == "Teacher")
            {
                lessons = await (from l in _context.Lessons
                                 join s in _context.Subjects on l.SubjectId equals s.Id
                                 where s.TeacherId.ToString() == userId && l.StartTime.Month == monthNo
                                 select l).ToListAsync();
            }
            else
            {
                lessons = await (from l in _context.Lessons
                                 join s in _context.Subjects.Include(x => x.Students) on l.SubjectId equals s.Id
                                 where s.Students.Any(x => x.Id.ToString() == userId) && l.StartTime.Month == monthNo
                                 select l).ToListAsync();
            }

            List<TrimmedLesson> trimmed = new List<TrimmedLesson>();
            foreach(Lesson lesson in lessons)
            {
                trimmed.Add(new TrimmedLesson(lesson.Id, lesson.StartTime, lesson.SubjectId, lesson.Code, lesson.CodeTime, lesson.CampusId));
            }

            return trimmed;
        }

        // DID STUDENT CHECK IN?: api/Lessons/CheckedIn/5
        [HttpGet("CheckedIn/{id}")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<bool>> DidStudentCheckIn(int id)
        {
            string userId;

            if (_mockUser == default)
                userId = User.FindFirst(ClaimTypes.Name)?.Value;
            else
                userId = _mockUser.Id.ToString();

            Lesson thisLesson = await _context.Lessons
                                    .Include(x => x.Students)
                                    .Where(x => x.Id == id)
                                    .FirstOrDefaultAsync();

            return thisLesson.Students.Any(x => x.Id.ToString() == userId);
        }

        // GET ALL CHECKED IN STUDENTS: api/Lessons/GetAllCheckedIn/5
        [HttpGet("GetAllCheckedIn/{id}")]
        [Authorize(Roles = "Teacher, Admin")]
        public async Task<ActionResult<List<TrimmedUser>>> GetAllCheckedIn(int id)
        {
            Lesson thisLesson = await _context.Lessons
                                    .Include(x => x.Students)
                                    .Where(x => x.Id == id)
                                    .FirstOrDefaultAsync();

            List<TrimmedUser> students = new List<TrimmedUser>();

            foreach(User student in thisLesson.Students)
            {
                students.Add(new TrimmedUser(student.Id, student.Email, student.FirstName, student.LastName, student.RoleId));
            }

            return students;
        }

        // GET CURRENT: api/Lessons/Current
        [HttpGet("Current")]
        [Authorize(Roles = "Student, Teacher")]
        public async Task<ActionResult<TrimmedLesson>> GetCurrentLesson()
        {
            string userRole;
            string userId;
            
            if(_mockUser == default)
            {
                userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                userId = User.FindFirst(ClaimTypes.Name)?.Value;
            }
            else
            {
                userRole = HelperFunctions.GetRoleName(_mockUser.RoleId);
                userId = _mockUser.Id.ToString();
            }

            List<Subject> subjects = new List<Subject>();

            if (userRole == "Teacher")
            {
                subjects = await _context.Subjects
                                        .Where(x => x.TeacherId.ToString() == userId)
                                        .ToListAsync();
            }

            if(userRole == "Student")
            {
                subjects = await _context.Subjects
                                        .Include(s => s.Students)
                                        .Where(x => x.Students.Any(x => x.Id.ToString() == userId))
                                        .ToListAsync();
            }

            List<int> subjectIds = subjects.Select(x => x.Id).ToList();
            List<int?> teacherIds = subjects.Select(x => x.TeacherId).ToList();

            List<Lesson> lessonsByTime = await _context.Lessons
                                .Where(x => subjectIds.Contains((int)x.SubjectId))
                                .OrderBy(x => x.StartTime)
                                .ToListAsync();

            Lesson lesson = lessonsByTime.Where(x => x.StartTime > DateTime.Now.AddHours(-1)).FirstOrDefault();

            List <User> users = await _context.Users.Where(x => x.RoleId == 1 && teacherIds.Contains(x.Id)).ToListAsync();

            Campus campus = await _context.Campuses.Where(x => x.Id == lesson.CampusId).FirstOrDefaultAsync();
            User teacher = (from s in subjects
                            join u in users on s.TeacherId equals u.Id
                            where s.Id == lesson.SubjectId
                            select u).FirstOrDefault();

            TrimmedLesson trimmed = new TrimmedLesson(lesson.Id, lesson.StartTime, lesson.SubjectId, lesson.Code, lesson.CodeTime, lesson.CampusId);
            trimmed.CampusName = campus.Name;
            trimmed.SubjectName = subjects.Where(x => x.Id == trimmed.SubjectId).First().Name;
            trimmed.TeacherName = teacher.FirstName + " " + teacher.LastName;

            return trimmed;
        }

        // GET: api/Lessons
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TrimmedLesson>>> GetLessons()
        {
            List<Lesson> lessons = await _context.Lessons.ToListAsync();
            List<Subject> subjects = await _context.Subjects
                                                .Include(s => s.Students)
                                                .ToListAsync();
            List<Campus> campuses = await _context.Campuses.ToListAsync();

            string userRole;
            string userId;

            if (_mockUser == default)
                userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            else
                userRole = HelperFunctions.GetRoleName(_mockUser.RoleId);


            var trimmedLessons = new List<TrimmedLesson>();

            if (userRole == "Student")
            {
                if(_mockUser == default)
                    userId = User.FindFirst(ClaimTypes.Name)?.Value;
                else
                    userId = _mockUser.Id.ToString();

                var availableLesssons = from l in lessons
                                        join s in subjects on l.SubjectId equals s.Id
                                        where s.Students.Any(x => x.Id.ToString() == userId)
                                        select l;

                foreach(Lesson lesson in availableLesssons)
                {
                    var newLesson = new TrimmedLesson(lesson.Id, lesson.StartTime, lesson.SubjectId, lesson.Code, lesson.CodeTime, lesson.CampusId);
                    newLesson.SubjectName = (from s in subjects
                                            where s.Id == newLesson.SubjectId
                                            select s.Name).FirstOrDefault();
                    newLesson.CampusName = campuses.FirstOrDefault(x => x.Id == lesson.CampusId).Name;
                    trimmedLessons.Add(newLesson);

                }

                return trimmedLessons;
            }

            var allLessons = await _context.Lessons.ToListAsync();
            foreach (Lesson lesson in allLessons)
            {
                var newLesson = new TrimmedLesson(lesson.Id, lesson.StartTime, lesson.SubjectId, lesson.Code, lesson.CodeTime, lesson.CampusId);
                newLesson.SubjectName = (from s in subjects
                                         where s.Id == newLesson.SubjectId
                                         select s.Name).FirstOrDefault();
                newLesson.CampusName = campuses.FirstOrDefault(x => x.Id == lesson.CampusId).Name;
                trimmedLessons.Add(newLesson);
            }

            return trimmedLessons;
        }

        // GET: api/Lessons/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Teacher, Admin")]
        public async Task<ActionResult<TrimmedLesson>> GetLesson(int id)
        {
            var lesson = await _context.Lessons.FindAsync(id);

            if (lesson == null)
            {
                return NotFound();
            }

            TrimmedLesson trimmedLesson = new TrimmedLesson(lesson.Id, lesson.StartTime, lesson.SubjectId, lesson.Code, lesson.CodeTime, lesson.CampusId);
            Subject subject = await _context.Subjects
                                        .Where(s => s.Id == trimmedLesson.SubjectId)
                                        .FirstOrDefaultAsync();
            Campus campus = await _context.Campuses
                                        .Where(c => c.Id == trimmedLesson.CampusId)
                                        .FirstOrDefaultAsync();
            trimmedLesson.SubjectName = subject.Name;
            trimmedLesson.CampusName = campus.Name;
            return trimmedLesson;
        }

        // GET BY SUBJECT: api/Lessons/BySubject/5
        [HttpGet("BySubject/{id}")]
        public async Task<ActionResult<IEnumerable<TrimmedLesson>>> GetLessonsBySubject(int id)
        {
            string userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            Subject subject = await _context.Subjects
                                                .Where(x => x.Id == id)
                                                .Include(s => s.Students)
                                                .FirstAsync();

            List<Lesson> lessons = await _context.Lessons
                                                .Where(x => x.SubjectId == id)
                                                .ToListAsync();

            List<Campus> campuses = await _context.Campuses.ToListAsync();

            if(userRole == "Student")
            {
                string userId = User.FindFirst(ClaimTypes.Name)?.Value;
                if (!subject.Students.Any(x => x.Id.ToString() == userId))
                {
                    return Unauthorized();
                }
            }

            List<TrimmedLesson> trimmedLessons = new List<TrimmedLesson>();
            foreach(Lesson lesson in lessons)
            {
                TrimmedLesson trimmedLesson = new TrimmedLesson(lesson.Id, lesson.StartTime, lesson.SubjectId, lesson.Code, lesson.CodeTime, lesson.CampusId);
                trimmedLesson.SubjectName = subject.Name;
                trimmedLesson.CampusName = campuses.FirstOrDefault(x => x.Id == lesson.CampusId).Name;
                trimmedLessons.Add(trimmedLesson);
            }

            return trimmedLessons;
        }

        // START ROLL CALL: api/Lesssons/StartRollCall/5
        [HttpPut("StartRollCall/{id}")]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<string>> StartRollCall(int id)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null)
            {
                return NotFound();
            }

            Subject subject = await _context.Subjects.FindAsync(lesson.SubjectId);
            if (subject == null)
            {
                return NotFound();
            }

            string userId = User.FindFirst(ClaimTypes.Name)?.Value;
            if(userId != subject.TeacherId.ToString())
            {
                return "You are not the teacher of this subject and therefore cannot start a roll call.";
            }
            
            if(lesson.Code != null || lesson.CodeTime != null)
            {
                return "This lesson has already had a roll call.";
            }

            lesson.Code = Logic.RollCallCodeGenerator.GenerateCode();
            lesson.CodeTime = DateTime.Now;

            _context.Entry(lesson).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LessonExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return "Roll call has started.";
        }

        // CHECK IN TO LESSON: api/Lessons/CheckIn/5
        [HttpPost("CheckIn/{id}")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<string>> CheckIn(int id, int code)
        {
            var lesson = await _context.Lessons.FindAsync(id);

            if (lesson == null)
            {
                return NotFound();
            }

            if(lesson.Code == null || lesson.CodeTime == null)
            {
                return BadRequest();
            }

            if (lesson.Code != code)
            {
                return "Incorrect code.";
            }

            TimeSpan span = (TimeSpan)(DateTime.Now - lesson.CodeTime);
            if (span.TotalMinutes > 10)
            {
                return "You can no longer check into this lesson.";
            }

            string userId = User.FindFirst(ClaimTypes.Name)?.Value;
            var noOfRowAffected = await _context.Database
                .ExecuteSqlRawAsync($"insert into attending_student (student_id, lesson_id) value({userId}, {lesson.Id}); ");

            if (noOfRowAffected == 0) return BadRequest();
            else return "You have checked in!";
        }

        // PUT: api/Lessons/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutLesson(int id, Lesson lesson)
        {
            if (id != lesson.Id)
            {
                return BadRequest();
            }

            _context.Entry(lesson).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LessonExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Lessons
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Lesson>> PostLesson(Lesson lesson)
        {
            _context.Lessons.Add(lesson);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLesson", new { id = lesson.Id }, lesson);
        }

        // DELETE: api/Lessons/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteLesson(int id)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null)
            {
                return NotFound();
            }

            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LessonExists(int id)
        {
            return _context.Lessons.Any(e => e.Id == id);
        }
    }
}
