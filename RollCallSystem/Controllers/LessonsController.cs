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

namespace RollCallSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LessonsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LessonsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Lessons
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TrimmedLesson>>> GetLessons()
        {
            List<Lesson> lessons = await _context.Lessons.ToListAsync();
            List<Subject> subjects = await _context.Subjects
                                                .Include(s => s.Students)
                                                .ToListAsync();

            string userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            var trimmedLessons = new List<TrimmedLesson>();

            if (userRole == "Student")
            {
                string userId = User.FindFirst(ClaimTypes.Name)?.Value;
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
                trimmedLessons.Add(newLesson);
            }

            return trimmedLessons;
        }

        // GET: api/Lessons/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Teacher, Admin")]
        public async Task<ActionResult<Lesson>> GetLesson(int id)
        {
            var lesson = await _context.Lessons.FindAsync(id);

            if (lesson == null)
            {
                return NotFound();
            }

            return lesson;
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
