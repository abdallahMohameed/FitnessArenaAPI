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
    public class workoutsController : ControllerBase
    {
        private readonly fitnessgarageContext _context;

        public workoutsController(fitnessgarageContext context)
        {
            _context = context;
        }

        // GET: api/workouts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<workout>>> Getworkouts()
        {
          if (_context.workouts == null)
          {
              return NotFound();
          }
            return await _context.workouts.Include(a=>a.programWorkouts).ToListAsync();
        }

        // GET: api/workouts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<workout>> Getworkout(int id)
        {
          if (_context.workouts == null)
          {
              return NotFound();
          }
            var workout = await _context.workouts.FindAsync(id);

            if (workout == null)
            {
                return NotFound();
            }

            return workout;
        }

        // PUT: api/workouts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //roles coach
        [HttpPut("{id}")]
        public async Task<IActionResult> Putworkout(int id, workout workout)
        {
            if (id != workout.workoutId)
            {
                return BadRequest();
            }

            _context.Entry(workout).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!workoutExists(id))
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

        // POST: api/workouts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<workout>> Postworkout(workout workout)
        {
          if (_context.workouts == null)
          {
              return Problem("Entity set 'fitnessgarageContext.workouts'  is null.");
          }
            _context.workouts.Add(workout);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Getworkout", new { id = workout.workoutId }, workout);
        }

        // DELETE: api/workouts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deleteworkout(int id)
        {
            if (_context.workouts == null)
            {
                return NotFound();
            }
            var workout = await _context.workouts.FindAsync(id);
            if (workout == null)
            {
                return NotFound();
            }

            _context.workouts.Remove(workout);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //get workouts by trainee ID
        [HttpGet("traineeworkout/{id}")]
        public ActionResult<List<programWorkout>> getTraineeWorkOutsById(int id)
        {
            user? trainee = _context.users.FirstOrDefault(a => a.userId == id);
            if (trainee == null || trainee.type !="trainee")
            {
            return NotFound("This is Not a TraineeId");   
            }

            List<int?> programIds = _context.traineeprograms.Where(a => a.traineeId == id).Select(a => a.programId).ToList();

            if(programIds.Count != 0)
            {
                List<programWorkout> programWorkouts = new List<programWorkout>();
                foreach (var programId in programIds)
                {
                    var workoutlist = _context.programWorkouts.Where(a=>a.programId == programId)
                        .Include(a=>a.workout)
                        .ToList();

                    programWorkouts.AddRange(workoutlist);

                }
                return Ok(new { ids = programIds, programWorkouts=programWorkouts});
            }
          
                return NotFound("No Programs For this Trainee");
        }

        private bool workoutExists(int id)
        {
            return (_context.workouts?.Any(e => e.workoutId == id)).GetValueOrDefault();
        }
    }
}
