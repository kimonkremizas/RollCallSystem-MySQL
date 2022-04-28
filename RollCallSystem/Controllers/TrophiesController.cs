#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RollCallSystem.Database;

namespace RollCallSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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

        // PUT: api/Trophies/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
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
        public async Task<ActionResult<Trophy>> PostTrophy(Trophy trophy)
        {
            _context.Trophies.Add(trophy);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTrophy", new { id = trophy.Id }, trophy);
        }

        // DELETE: api/Trophies/5
        [HttpDelete("{id}")]
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
