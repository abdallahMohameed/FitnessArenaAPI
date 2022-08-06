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
    public class postsCommentsController : ControllerBase
    {
        private readonly fitnessgarageContext _context;

        public postsCommentsController(fitnessgarageContext context)
        {
            _context = context;
        }

        // GET: api/postsComments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<postsComment>>> GetpostsComments()
        {
          if (_context.postsComments == null)
          {
              return NotFound();
          }
            return await _context.postsComments.ToListAsync();
        }

        // GET: api/postsComments/5
        [HttpGet("{id}")]

        //Id For Post Not For comment ==>to get this post Comments
        public async Task<ActionResult> GetpostsComment(int id)
        {
            if (_context.posts.FirstOrDefault(o => o.postId == id) == null)
            {
                return NotFound("this post doesn't exist");
            }
            var postComments=_context.postsComments.Where(o => o.postId == id)
                .Select(x => new { x.Content, x.user.fname, x.user.lname ,x.user.type,x.postId})
                .ToList();

            if (postComments == null)
            {
                return NotFound();
            }

            return Ok(postComments);
        }

        // PUT: api/postsComments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutpostsComment(int id, postsComment postsComment)
        {
            if (id != postsComment.commentId)
            {
                return BadRequest();
            }

            postsComment oldcomment = await _context.postsComments.FindAsync(id);

            if (oldcomment == null)
            {
                return NotFound();
            }
            else
            {
                oldcomment.Content = postsComment.Content;
            }


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!postsCommentExists(id))
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

        // POST: api/postsComments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<postsComment>> PostpostsComment(postsComment postsComment)
        {
          if (_context.postsComments == null)
          {
              return Problem("Entity set 'fitnessgarageContext.postsComments'  is null.");
          }
            _context.postsComments.Add(postsComment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetpostsComment", new { id = postsComment.commentId }, postsComment);
        }

        // DELETE: api/postsComments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletepostsComment(int id)
        {
            if (_context.postsComments == null)
            {
                return NotFound();
            }
            var postsComment = await _context.postsComments.FindAsync(id);
            if (postsComment == null)
            {
                return NotFound();
            }

            _context.postsComments.Remove(postsComment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool postsCommentExists(int id)
        {
            return (_context.postsComments?.Any(e => e.commentId == id)).GetValueOrDefault();
        }
    }
}
