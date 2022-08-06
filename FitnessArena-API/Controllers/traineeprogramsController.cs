using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessArena_API.Models;

namespace FitnessArena_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class traineeprogramsController : ControllerBase
    {
        private readonly fitnessgarageContext _context;

        public traineeprogramsController(fitnessgarageContext context)
        {
            _context = context;
        }

        // GET: api/traineeprograms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<traineeprogram>>> Gettraineeprograms()
        {
          if (_context.traineeprograms == null)
          {
              return NotFound();
          }
            return await _context.traineeprograms.ToListAsync();
        }

        // GET: api/traineeprograms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<traineeprogram>> Gettraineeprogram(int id)
        {
          if (_context.traineeprograms == null)
          {
              return NotFound();
          }
            var traineeprogram = await _context.traineeprograms.FindAsync(id);

            if (traineeprogram == null)
            {
                return NotFound();
            }

            return traineeprogram;
        }
        [HttpGet("programstrainee/{id}")]
        public async Task<ActionResult> getProgram(int id)
        {
            var programsId = await _context.traineeprograms
                .Where(p => p.traineeId == id)
                .Select(c => c.programId).ToListAsync();
            List<program> programs = await _context.programs
                .Where(p => programsId.Contains(p.programId))
                .Include(o => o.traineeprograms).ToListAsync();

            if (programs == null)
            {
                return NotFound();
            }

            return Ok(programs);
        }

        // PUT: api/traineeprograms/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Puttraineeprogram(int id, traineeprogram traineeprogram)
        {
            if (id != traineeprogram.traiProgID)
            {
                return BadRequest();
            }

            _context.Entry(traineeprogram).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!traineeprogramExists(id))
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

        // POST: api/traineeprograms
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<traineeprogram>> Posttraineeprogram(traineeprogram traineeprogram)
        {
          if (_context.traineeprograms == null)
          {
              return Problem("Entity set 'fitnessgarageContext.traineeprograms'  is null.");
          }
            _context.traineeprograms.Add(traineeprogram);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Gettraineeprogram", new { id = traineeprogram.traiProgID }, traineeprogram);
        }

        // DELETE: api/traineeprograms/5
        [HttpDelete("unassign/{id}")]
        public async Task<IActionResult> Deletetraineeprogram(int id)
        {
            

            var traineeprogram = await _context.traineeprograms.FindAsync(id);
            if (traineeprogram == null)
            {
                return NotFound();
            }

            _context.traineeprograms.Remove(traineeprogram);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("traineeprogram/{id}")]
        public async Task<ActionResult<List<traineeprogram>>> Gettrainee(int id)
            //id for tainee id
        {

            List<traineeprogram> traineeprograms = _context.traineeprograms.Where(a => a.traineeId ==id).Include(o=>o.program).ToList();

            if (traineeprograms == null)
            {
                return NotFound();
        }

            return traineeprograms;
        }
        private bool traineeprogramExists(int id)
        {
            return (_context.traineeprograms?.Any(e => e.traiProgID == id)).GetValueOrDefault();
        }
    }
}
