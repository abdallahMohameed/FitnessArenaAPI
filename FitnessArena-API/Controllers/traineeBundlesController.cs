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
    public class traineeBundlesController : ControllerBase
    {
        private readonly fitnessgarageContext _context;

        public traineeBundlesController(fitnessgarageContext context)
        {
            _context = context;
        }

        // GET: api/traineeBundles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<traineeBundle>>> GettraineeBundles()
        {
          if (_context.traineeBundles == null)
          {
              return NotFound();
          }
            return await _context.traineeBundles.ToListAsync();
        }
        [HttpGet("traineeBundle/{id}")]
        public async Task<ActionResult> GtraineeBundles(int id)
        {
             traineeBundle bundle = await _context.traineeBundles.FirstOrDefaultAsync(a => a.traineeId == id);
            if(bundle == null)
            {
                return NotFound();
            }
            bundle bundles = await _context.bundles.FirstOrDefaultAsync(b =>
                b.bundleId == bundle.bundleId);
            if(bundles == null)
            {
                return NotFound();
            }
            return Ok(bundles);
            
        }


        // POST: api/traineeBundles
        [HttpPost]
        public async Task<ActionResult<traineeBundle>> PosttraineeBundle(traineeBundle traineeBundle)
        {
         
            var subiscribedBundle = _context.bundles.Where(o=>o.bundleId == traineeBundle.bundleId).FirstOrDefault();
            if (subiscribedBundle == null)
            {
                return NotFound("this bundle not exists");
            }

            traineeBundle.expiryDate= DateTime.Now.AddDays(subiscribedBundle.durationDays);
            _context.traineeBundles.Add(traineeBundle);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (traineeBundleExists(traineeBundle.traineeId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GettraineeBundle", new { id = traineeBundle.traineeId }, traineeBundle);
        }

        // DELETE: api/traineeBundles/5
        //Role Coach
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletetraineeBundle(int id)
        {
            if (_context.traineeBundles == null)
            {
                return NotFound();
            }
            var traineeBundle = await _context.traineeBundles.FindAsync(id);
            if (traineeBundle == null)
            {
                return NotFound();
            }

            _context.traineeBundles.Remove(traineeBundle);
            await _context.SaveChangesAsync();

            return NoContent();
        }



        private bool traineeBundleExists(int id)
        {
            return (_context.traineeBundles?.Any(e => e.traineeId == id)).GetValueOrDefault();
        }

        [HttpGet("hasbundle")]
        public async Task<ActionResult<bool>> hasBundle(int traineeId)
        {

            var hasBubdle = _context.traineeBundles.Where(a=>a.traineeId == traineeId).FirstOrDefault();
            if (hasBubdle == null)
            {
                return false;
            }
            else
            {
                return true;

            }

        }
        [HttpDelete("unsubscribe/{id}")]
        public async Task<IActionResult> unssubscribe(int traineeId)
        {
            //id for trainee
          
            var traineeBundle = await _context.traineeBundles.FindAsync(traineeId);
            if (traineeBundle == null)
            {
                return NotFound();
            }

            _context.traineeBundles.Remove(traineeBundle);
            await _context.SaveChangesAsync();

            return NoContent();
        }





    }

}
