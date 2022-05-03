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
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        [Authorize(Roles = "Teacher, Admin")]
        public async Task<ActionResult<IEnumerable<TrimmedUser>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            List<TrimmedUser> trimmedUsers = new List<TrimmedUser>();

            foreach(User user in users)
            {
                trimmedUsers.Add(new TrimmedUser(user.Id, user.Email, user.FirstName, user.LastName, user.RoleId));
            }

            return trimmedUsers;
        }

        // GET: api/Users/Teachers
        [HttpGet("Teachers")]
        public async Task<ActionResult<IEnumerable<TrimmedUser>>> GetTeachers()
        {
            var users = await _context.Users.Where(x => x.RoleId == 1).ToListAsync();
            List<TrimmedUser> trimmedUsers = new List<TrimmedUser>();

            foreach (User user in users)
            {
                trimmedUsers.Add(new TrimmedUser(user.Id, user.Email, user.FirstName, user.LastName, user.RoleId));
            }

            return trimmedUsers;
        }

        // GET: api/Users/Students
        [HttpGet("Students")]
        [Authorize(Roles = "Teacher, Admin")]
        public async Task<ActionResult<IEnumerable<TrimmedUser>>> GetStudents()
        {
            var users = await _context.Users.Where(x => x.RoleId == 0).ToListAsync();
            List<TrimmedUser> trimmedUsers = new List<TrimmedUser>();

            foreach (User user in users)
            {
                trimmedUsers.Add(new TrimmedUser(user.Id, user.Email, user.FirstName, user.LastName, user.RoleId));
            }

            return trimmedUsers;
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Teacher, Admin")]
        public async Task<ActionResult<TrimmedUser>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return new TrimmedUser(user.Id, user.Email, user.FirstName, user.LastName, user.RoleId);
        }

        // GET: api/Users/Self
        [HttpGet("Self")]
        public async Task<ActionResult<TrimmedUser>> GetSelf()
        {
            string userId = User.FindFirst(ClaimTypes.Name)?.Value;
            User user = await _context.Users.FindAsync(Convert.ToInt32(userId));

            if (user == null)
            {
                return NotFound();
            }

            return new TrimmedUser(user.Id, user.Email, user.FirstName, user.LastName, user.RoleId);
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
