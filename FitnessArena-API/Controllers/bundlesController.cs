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
    public class bundlesController : ControllerBase
    {
        private readonly fitnessgarageContext _context;

        public bundlesController(fitnessgarageContext context)
        {
            _context = context;
        }

        // GET: api/bundles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<bundle>>> Getbundles()
        {
          if (_context.bundles == null)
          {
              return NotFound();
          }
            return await _context.bundles.ToListAsync();
        }

        // GET: api/bundles/5
        [HttpGet("{id}")]
        //get all bundles by a coach with this ID
        public async Task<ActionResult<List<bundle>>> Getbundle(int id)
        {

           user coach= await _context.users.FindAsync(id);
            if (coach == null)
            {
                return NotFound("this coach is not found");
            }
            
            
                if(coach.type != "coach")
                {
                    return BadRequest("a Trainee can't Offer bundles");
                }
            
            var bundles =  _context.bundles.Where(o=>o.coachId==id).ToList();

            if (bundles == null)
            {
                return NotFound();
            }

            return bundles;
        }

        // PUT: api/bundles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Putbundle(int id, bundle bundle)
        {
            if (id != bundle.bundleId)
            {
                return BadRequest();
            }

            _context.Entry(bundle).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!bundleExists(id))
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

        // POST: api/bundles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<bundle>> Postbundle(bundle bundle)
        {
          if (_context.bundles == null)
          {
              return Problem("Entity set 'fitnessgarageContext.bundles'  is null.");
          }
            _context.bundles.Add(bundle);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Getbundle", new { id = bundle.bundleId }, bundle);
        }

        // DELETE: api/bundles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletebundle(int id)
        {
            if (_context.bundles == null)
            {
                return NotFound();
            }
            var bundle = await _context.bundles.FindAsync(id);
            if (bundle == null)
            {
                return NotFound();
            }

            _context.bundles.Remove(bundle);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool bundleExists(int id)
        {
            return (_context.bundles?.Any(e => e.bundleId == id)).GetValueOrDefault();
        }

        [HttpDelete("/rateb")]
        public async Task<IActionResult> Deletebund(int id)
        {
            if (_context.bundles == null)
            {
                return NotFound();
            }
            var bundle = await _context.bundles.FindAsync(id);
            if (bundle == null)
            {
                return NotFound();
            }

            _context.bundles.Remove(bundle);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
