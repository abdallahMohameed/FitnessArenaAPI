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
    public class postsSharesController : ControllerBase
    {
        private readonly fitnessgarageContext _context;

        public postsSharesController(fitnessgarageContext context)
        {
            _context = context;
        }

       
       


        // POST: api/postsShares
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<postsShare>> PostpostsShare(postsShare postsShare)
        {

            postsShare? checkForshare = _context.postsShares.FirstOrDefault(x => x.postId == postsShare.postId && x.userId == postsShare.userId);
            if (checkForshare == null)
            {
                _context.postsShares.Add(postsShare);
                await _context.SaveChangesAsync();
            }
            else
            {
                try
                {
                    _context.postsShares.Remove(checkForshare);
                    await _context.SaveChangesAsync();
                    return Ok("this user removed his removed from this post");
                }
                catch
                {

                    return NotFound("data base Error");
                }
            }

            return Ok("this user shared this post");
        }

        // DELETE: api/postsShares/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletepostsShare(int id)
        {
            if (_context.postsShares == null)
            {
                return NotFound();
            }
            var postsShare = await _context.postsShares.FindAsync(id);
            if (postsShare == null)
            {
                return NotFound();
            }

            _context.postsShares.Remove(postsShare);
            await _context.SaveChangesAsync();

            return NoContent();
        }



        //get post share count
        [HttpGet("count/{id}")]
        public ActionResult<int> postSharesCount(int id)
        {
            //id==>for post id

            if (_context.posts.FirstOrDefault(o => o.postId == id) == null)
            {
                return NotFound("this post doesn't exist");
            }
            var postShareCount = _context.postsShares.Where(o => o.postId == id).Count();

            if (postShareCount == 0)
            {
                return 0;
            }

            return postShareCount;
        }



        [HttpGet("postshares/{id}")]
        public ActionResult<List<user>> getPostshares(int id)
        {

            //id==>for post id
            if (_context.posts.FirstOrDefault(o=>o.postId==id) == null)
            {
                return NotFound("this post doesn't exist");
            }
            var postShares = _context.postsShares.Include(u => u.user).Where(o => o.postId == id);

            if (postShares == null)
            {
                return NotFound("No users Shared This Post");
            }
            else
            {
                List<user> users = new List<user>();
                foreach (var d in postShares)
                {
                    users.Add(d.user);
                }
                return users;
            }

        }
        private bool postsShareExists(int id)
        {
            return (_context.postsShares?.Any(e => e.shareId == id)).GetValueOrDefault();
        }

        //get post likes by a single user
        [HttpGet("getuserShare/{id}")]
        public ActionResult<List<int?>> userShares(int id)
        {
            //id==>for user id
            List<int?>  userShare = _context.postsShares.Where(o => o.userId == id).Select(u => u.postId).ToList();
            if (userShare == null)
            {
                return NotFound("this user likes no post");
            }
            return Ok(userShare);
        }
    }
}
