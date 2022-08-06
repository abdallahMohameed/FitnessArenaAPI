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
    public class postsLikesController : ControllerBase
    {
        private readonly fitnessgarageContext _context;

        public postsLikesController(fitnessgarageContext context)
        {
            _context = context;
        }

    




        // POST: api/postsLikes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<postsLike>> PostpostsLike(postsLike postsLike)
        {
            postsLike? checkForLike = _context.postsLikes.FirstOrDefault(x => x.postId == postsLike.postId && x.userId == postsLike.userId);
            if (checkForLike == null)
            {
                _context.postsLikes.Add(postsLike);
                await _context.SaveChangesAsync();
            }
            else
            {
                try
                {
                    _context.postsLikes.Remove(checkForLike);
                    await _context.SaveChangesAsync();
                    return Ok("this user removed his like from this post");
                }
                catch
                {

                    return NotFound("data base Error");
                }

            }



            return Ok("this user liked this post");
        }

        // DELETE: api/postsLikes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletepostsLike(int id)
        {
            if (_context.postsLikes == null)
            {
                return NotFound();
            }
            var postsLike = await _context.postsLikes.FindAsync(id);
            if (postsLike == null)
            {
                return NotFound();
            }

            _context.postsLikes.Remove(postsLike);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        //get post likes count
        [HttpGet("count/{id}")]
        public ActionResult<int> postLikesCount(int id)
        {
            //id==>for post id

            if (_context.posts == null)
            {
                return NotFound();
            }
            var postLikesCount = _context.postsLikes.Where(o => o.postId == id).Count();

            if (postLikesCount == 0)
            {
                return 0;
            }

            return postLikesCount;
        }


        [HttpGet("postlikers/{id}")]
        public ActionResult<List<user>> getPostLikers(int id)
        {

            //id==>for post id
            if (_context.posts.FirstOrDefault(o => o.postId == id) == null)
            {
                return NotFound("this post doesn't exist");
            }
            var postLikers = _context.postsLikes.Include(u => u.user).Where(o => o.postId == id);

            if (postLikers == null)
            {
                return NotFound("No users Likes This Post");
            }
            else
            {
                List<user> users = new List<user>();
                foreach (var d in postLikers)
                {
                    users.Add(d.user);
                }
                return users;
            }

        }


        private bool postsLikeExists(int id)
        {
            return (_context.postsLikes?.Any(e => e.likeId == id)).GetValueOrDefault();
        }

        //get post likes by a single user
        [HttpGet("getuserlikes/{id}")]
        public ActionResult<int?> userlikes(int id)
        {
            //id==>for user id
            var userlikes = _context.postsLikes.Where(o => o.userId == id).Select(u => u.postId).ToList();
            if (userlikes == null)
            {
                return NotFound("this user likes no post");
            }
            return Ok(userlikes);
        }
    }
}
