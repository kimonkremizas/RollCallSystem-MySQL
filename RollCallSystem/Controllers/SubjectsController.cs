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
    public class SubjectsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SubjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Subjects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TrimmedSubject>>> GetSubjects()
        {
            var subjects = await _context.Subjects
                .Include(s => s.Students)
                .ToListAsync();

            var teachers = await _context.Users
                .Where(x => x.RoleId == 1)
                .ToListAsync();

            string userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            if(userRole == "Student")
            {
                string userId = User.FindFirst(ClaimTypes.Name)?.Value;
                subjects = subjects.Where(x => x.Students.Any(y => y.Id.ToString() == userId)).ToList();
            }

            List<TrimmedSubject> trimmedSubjects = new List<TrimmedSubject>();
                
            foreach(Subject subject in subjects)
            {
                TrimmedSubject trimmedSubject = new TrimmedSubject(subject.Id, subject.Name, subject.TeacherId);
                User theTeacher = teachers.FirstOrDefault(x => x.Id == subject.TeacherId);
                trimmedSubject.TeacherName = theTeacher.FirstName + " " + theTeacher.LastName;
                trimmedSubjects.Add(trimmedSubject);
            }

            return trimmedSubjects;
        }

        // GET: api/Subjects/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TrimmedSubject>> GetSubject(int id)
        {
            Subject subject;
            var subjects = await _context.Subjects
                .Include(s => s.Students)
                .ToListAsync();

            var teachers = await _context.Users
                .Where(x => x.RoleId == 1)
                .ToListAsync();

            subject = subjects.FirstOrDefault(x => x.Id == id);
            if (subject == null)
            {
                return NotFound();
            }

            string userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole == "Student")
            {
                string userId = User.FindFirst(ClaimTypes.Name)?.Value;
                if (!subject.Students.Any(x => x.Id.ToString() == userId))
                {
                    return Unauthorized();
                }
            }

            TrimmedSubject trimmedSubject = new TrimmedSubject(subject.Id, subject.Name, subject.TeacherId);
            User theTeacher = teachers.FirstOrDefault(x => x.Id == subject.TeacherId);
            trimmedSubject.TeacherName = theTeacher.FirstName + " " + theTeacher.LastName;
            return trimmedSubject;
        }

        // GET BY STUDENT: api/Subjects/ByStudent/5
        [HttpGet("ByStudent/{id}")]
        [Authorize(Roles = "Teacher, Admin")]
        public async Task<ActionResult<List<TrimmedSubject>>> GetSubjectByStudent(int id)
        {
            var subjects = await _context.Subjects
                .Include(s => s.Students)
                .ToListAsync();

            var teachers = await _context.Users
                .Where(x => x.RoleId == 1)
                .ToListAsync();

            subjects = subjects.Where(x => x.Students.Any(y => y.Id == id)).ToList();

            List<TrimmedSubject> trimmedSubjects = new List<TrimmedSubject>();
            foreach(Subject subject in subjects)
            {
                TrimmedSubject trimmedSubject = new TrimmedSubject(subject.Id, subject.Name, subject.TeacherId);
                User theTeacher = teachers.FirstOrDefault(x => x.Id == subject.TeacherId);
                trimmedSubject.TeacherName = theTeacher.FirstName + " " + theTeacher.LastName;
                trimmedSubjects.Add(trimmedSubject);
            }

            return trimmedSubjects;
        }

        // GET BY TEACHER: api/Subjects/ByTeacher/5
        [HttpGet("ByTeacher/{id}")]
        [Authorize(Roles = "Teacher, Admin")]
        public async Task<ActionResult<List<TrimmedSubject>>> GetSubjectByTeacher(int id)
        {
            var subjects = await _context.Subjects
                .Include(s => s.Students)
                .ToListAsync();

            var teachers = await _context.Users
                .Where(x => x.RoleId == 1)
                .ToListAsync();

            subjects = subjects.Where(x => x.TeacherId == id).ToList();

            List<TrimmedSubject> trimmedSubjects = new List<TrimmedSubject>();
            foreach (Subject subject in subjects)
            {
                TrimmedSubject trimmedSubject = new TrimmedSubject(subject.Id, subject.Name, subject.TeacherId);
                User theTeacher = teachers.FirstOrDefault(x => x.Id == subject.TeacherId);
                trimmedSubject.TeacherName = theTeacher.FirstName + " " + theTeacher.LastName;
                trimmedSubjects.Add(trimmedSubject);
            }

            return trimmedSubjects;
        }

        // PUT: api/Subjects/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutSubject(int id, Subject subject)
        {
            if (id != subject.Id)
            {
                return BadRequest();
            }

            _context.Entry(subject).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubjectExists(id))
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

        // POST: api/Subjects
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Subject>> PostSubject(Subject subject)
        {
            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSubject", new { id = subject.Id }, subject);
        }

        // DELETE: api/Subjects/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
            {
                return NotFound();
            }

            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SubjectExists(int id)
        {
            return _context.Subjects.Any(e => e.Id == id);
        }
    }
}
