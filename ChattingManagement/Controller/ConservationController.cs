using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entity;

namespace ChattingManagement.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConservationController : ControllerBase
    {
        private readonly RallyWaveContext _context;

        public ConservationController(RallyWaveContext context)
        {
            _context = context;
        }

        // GET: api/Conservation
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Conservation>>> GetConservations()
        {
            return await _context.Conservations.ToListAsync();
        }

        // GET: api/Conservation/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Conservation>> GetConservation(int id)
        {
            var conservation = await _context.Conservations.FindAsync(id);

            if (conservation == null)
            {
                return NotFound();
            }

            return conservation;
        }

        // PUT: api/Conservation/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutConservation(int id, Conservation conservation)
        {
            if (id != conservation.ConservationId)
            {
                return BadRequest();
            }

            _context.Entry(conservation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConservationExists(id))
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

        // POST: api/Conservation
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Conservation>> PostConservation(Conservation conservation)
        {
            _context.Conservations.Add(conservation);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetConservation", new { id = conservation.ConservationId }, conservation);
        }

        // DELETE: api/Conservation/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConservation(int id)
        {
            var conservation = await _context.Conservations.FindAsync(id);
            if (conservation == null)
            {
                return NotFound();
            }

            _context.Conservations.Remove(conservation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ConservationExists(int id)
        {
            return _context.Conservations.Any(e => e.ConservationId == id);
        }
    }
}
