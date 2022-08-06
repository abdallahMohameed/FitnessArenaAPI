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
    public class inbodiesController : ControllerBase
    {
        private readonly fitnessgarageContext _context;

        public inbodiesController(fitnessgarageContext context)
        {
            _context = context;
        }

        // GET: api/inbodies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<inbody>>> Getinbodies()
        {
          if (_context.inbodies == null)
          {
              return NotFound();
          }
            return await _context.inbodies.ToListAsync();
        }

        // GET: api/inbodies/trainee/5
        [HttpGet("trainee/{id}")]
        //
        public async Task<ActionResult<List<inbody>>> Getinbody(int id)
        {
            var trainee = await _context.users.FindAsync(id);
            if (trainee == null)
            {
                return NotFound();
            }

            List<inbody> listOfInBodies = _context.inbodies.Where(o =>o.traineeId ==id).ToList();

            if (listOfInBodies == null)
            {
                return NotFound("this Trainee has not InBodies");
            }

            return listOfInBodies;
        }

        // PUT: api/inbodies/5
       // trinee Role
        [HttpPut("{id}")]
        public async Task<IActionResult> Putinbody(int id, inbody inbody)
        {
            if (id != inbody.inbodyId)
            {
                return BadRequest();
            }

            _context.Entry(inbody).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!inbodyExists(id))
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

        // POST: api/inbodies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<inbody>> Postinbody(inbody inbody)
        {
         
            _context.inbodies.Add(inbody);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Getinbody", new { id = inbody.inbodyId }, inbody);
        }

        // DELETE: api/inbodies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deleteinbody(int id)
        {
            if (_context.inbodies == null)
            {
                return NotFound();
            }
            var inbody = await _context.inbodies.FindAsync(id);
            if (inbody == null)
            {
                return NotFound();
            }

            _context.inbodies.Remove(inbody);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool inbodyExists(int id)
        {
            return (_context.inbodies?.Any(e => e.inbodyId == id)).GetValueOrDefault();
        }
    }
}
