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
    public class programWorkoutsController : ControllerBase
    {
        private readonly fitnessgarageContext _context;

        public programWorkoutsController(fitnessgarageContext context)
        {
            _context = context;
        }

        // GET: api/programWorkouts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<programWorkout>>> GetprogramWorkouts()
        {
          if (_context.programWorkouts == null)
          {
              return NotFound();
          }
            return await _context.programWorkouts.ToListAsync();
        }

        // GET: api/programWorkouts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<programWorkout>> GetprogramWorkout(int id)
        {
          if (_context.programWorkouts == null)
          {
              return NotFound();
          }
            var programWorkout = await _context.programWorkouts.FindAsync(id);

            if (programWorkout == null)
            {
                return NotFound();
            }

            return programWorkout;
        }

        // PUT: api/programWorkouts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutprogramWorkout(int id, programWorkout programWorkout)
        {
            if (id != programWorkout.progWork)
            {
                return BadRequest();
            }

            _context.Entry(programWorkout).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!programWorkoutExists(id))
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

        // POST: api/programWorkouts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<programWorkout>> PostprogramWorkout(programWorkout programWorkout)
        {
          if (_context.programWorkouts == null)
          {
              return Problem("Entity set 'fitnessgarageContext.programWorkouts'  is null.");
          }
            _context.programWorkouts.Add(programWorkout);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetprogramWorkout", new { id = programWorkout.progWork }, programWorkout);
        }

        // DELETE: api/programWorkouts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteprogramWorkout(int id)
        {
            if (_context.programWorkouts == null)
            {
                return NotFound();
            }
            var programWorkout = await _context.programWorkouts.FindAsync(id);
            if (programWorkout == null)
            {
                return NotFound();
            }

            _context.programWorkouts.Remove(programWorkout);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("getWorkoutByProgId/{id}")]
        public async Task<ActionResult<List<programWorkout>>> GetWorkout(int id)
        //id for program id
        {

            List<programWorkout> traineeprograms = _context.programWorkouts.Where(a => a.programId == id).Include(o => o.workout).ToList();

            if (traineeprograms == null)
            {
                return NotFound();
            }

            return traineeprograms;
        }
        private bool programWorkoutExists(int id)
        {
            return (_context.programWorkouts?.Any(e => e.progWork == id)).GetValueOrDefault();
        }
    }
}
