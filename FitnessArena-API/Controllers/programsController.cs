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
    public class programsController : ControllerBase
    {
        private readonly fitnessgarageContext _context;

        public programsController(fitnessgarageContext context)
        {
            _context = context;
        }

        // GET: api/programs/5
        [HttpGet("{id}")]
        //Get all coach progrmas
        public async Task<ActionResult<List<program>>> Getprogram(int id)
        {
            user coach = await _context.users.FindAsync(id);
            if (coach == null)
            {
                return NotFound("this coach is not found");
            }


            if (coach.type != "coach")
            {
                return BadRequest("a Trainee can't Offer Programs");
            }

            var progrmams = _context.programs.Where(o => o.coachId == id).ToList();

            if (progrmams == null)
            {
                return NotFound("this Coach has not programs");
            }

            return progrmams;
        }
        [HttpGet("workouts/{id}")]
        public async Task<IActionResult> progWork(int id)
        {
            program prog = await _context.programs.FindAsync(id);
            if(prog == null)
            {
                return BadRequest();
            }
            var workoutsId = await _context.programWorkouts
                .Where(a => a.programId == id)
                .Select(a => a.workoutId).ToListAsync();
            List<workout> proWorkOut = await _context.workouts
                .Where(w => workoutsId.Contains(w.workoutId))
                .ToListAsync();
            return Ok(proWorkOut);
        }

        [HttpGet("showone/{id}")]
        public async Task<IActionResult> showOne(int id)
        {
            program prog = await _context.programs
                .Where(a => a.programId == id)
                .Include(a => a.programWorkouts)
                .FirstOrDefaultAsync();
            if (prog == null)
            {
                return BadRequest("NO Program has This ID");
            }
            return Ok(prog);
        }

        // PUT: api/programs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Putprogram(int id, program program)
        {
            program prog = await _context.programs.FindAsync(id);
            if (prog == null)
            {
                return BadRequest();
            }
            prog.target = program.target;
            prog.progDecription = program.progDecription;
            prog.name = program.name;
            prog.programWorkouts = program.programWorkouts;
           
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!programExists(id))
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

        // POST: api/programs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<program>> Postprogram(program program)
        {
          if (_context.programs == null)
          {
              return Problem("Entity set 'fitnessgarageContext.programs'  is null.");
          }
            _context.programs.Add(program);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Getprogram", new { id = program.programId }, program);
        }

        // DELETE: api/programs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deleteprogram(int id)
        {
            if (_context.programs == null)
            {
                return NotFound();
            }
            var program = await _context.programs.FindAsync(id);
            if (program == null)
            {
                return NotFound();
            }

            _context.programs.Remove(program);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool programExists(int id)
        {
            return (_context.programs?.Any(e => e.programId == id)).GetValueOrDefault();
        }
    }
}
