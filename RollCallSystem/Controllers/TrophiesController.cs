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
using RollCallSystem.Database;

namespace RollCallSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TrophiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TrophiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Trophies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Trophy>>> GetTrophies()
        {
            return await _context.Trophies.ToListAsync();
        }

        // GET: api/Trophies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Trophy>> GetTrophy(int id)
        {
            var trophy = await _context.Trophies.FindAsync(id);

            if (trophy == null)
            {
                return NotFound();
            }

            return trophy;
        }

        // GET BY STUDENT: api/Trophies/ByStudent/5
        [HttpGet("ByStudent/{id}")]
        public async Task<ActionResult<IEnumerable<Trophy>>> GetTrophiesByStudent(int id)
        {
            string userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole == "Student")
            {
                string userId = User.FindFirst(ClaimTypes.Name)?.Value;
                if (userId != id.ToString()) return Unauthorized();
            }

            var student = await _context.Users
                .Where(x => x.Id == id && x.RoleId == 0)
                .Include(x => x.Trophies)
                .FirstAsync();

            List<Trophy> trophies = new List<Trophy>();
            foreach(Trophy trophy in student.Trophies)
            {
                trophy.Students = null;
                trophies.Add(trophy);
            }
            return trophies;
        }

        //ASSIGN TROPHY TO STUDENT: api/Trophies/Assign/1/1
        [HttpPost("Assign/{trophyId}/{studentId}")]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<string>> AssignTrophy(int trophyId, int studentId)
        {
            var student = await _context.Users
                .Where(x => x.Id == studentId && x.RoleId == 0)
                .Include(x => x.Trophies)
                .FirstAsync();

            if(student == null)
            {
                return NotFound();
            }

            var trophy = await _context.Trophies.FindAsync(trophyId);

            if(trophy == null)
            {
                return NotFound();
            }

            if(student.Trophies.Any(x => x.Id == trophyId))
            {
                return "This student already has this trophy.";
            }

            if((bool)trophy.Automatic)
            {
                return "This trophy cannot be assigned manually.";
            }

            var noOfRowAffected = await _context.Database
                .ExecuteSqlRawAsync($"insert into student_trophies (student_id, trophy_id) value({student.Id}, {trophy.Id}); ");

            if (noOfRowAffected == 0) return BadRequest();
            else return "Trophy given!";
        }

        // PUT: api/Trophies/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutTrophy(int id, Trophy trophy)
        {
            if (id != trophy.Id)
            {
                return BadRequest();
            }

            _context.Entry(trophy).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TrophyExists(id))
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

        // POST: api/Trophies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Trophy>> PostTrophy(Trophy trophy)
        {
            _context.Trophies.Add(trophy);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTrophy", new { id = trophy.Id }, trophy);
        }

        // DELETE: api/Trophies/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTrophy(int id)
        {
            var trophy = await _context.Trophies.FindAsync(id);
            if (trophy == null)
            {
                return NotFound();
            }

            _context.Trophies.Remove(trophy);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TrophyExists(int id)
        {
            return _context.Trophies.Any(e => e.Id == id);
        }
    }
}
