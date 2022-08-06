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
    public class postsController : ControllerBase
    {
        private readonly fitnessgarageContext _context;

        public postsController(fitnessgarageContext context)
        {
            _context = context;
        }

        // GET: api/posts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<post>>> Getposts()
        {
            if (_context.posts == null)
            {
                return NotFound();
            }
            List<post> posts = await _context.posts.Include(c => c.postsComments).Include(a=>a.user).ToListAsync();

            foreach(post post in posts)
            {
                post.numberOfLikes = _context.postsLikes.Where(o=>o.postId == post.postId).Count(); 
                post.numberOfShares = _context.postsShares.Where(o=>o.postId == post.postId).Count();
                post.numberOfComments = _context.postsComments.Where(o=>o.postId == post.postId).Count();
            }
            return Ok(posts);
        }

        // GET: api/posts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<post>> Getpost(int id)
        {
            if (_context.posts == null)
            {
                return NotFound();
            }
            var post = await _context.posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            return post;
        }

        // PUT: api/posts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Putpost(int id, post post)
        {
            if (id != post.postId)
            {
                return BadRequest();
            }

             post oldpost = await _context.posts.FindAsync(id);
            if(oldpost == null)
            {
                return NotFound();
            }
            else
            {
                oldpost.content= post.content;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!postExists(id))
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

        // POST: api/posts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<post>> Addpost(post post)
        {
            if (_context.posts == null)
            {
                return Problem("Entity set 'fitnessgarageContext.posts'  is null.");
            }
            _context.posts.Add(post);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Getpost", new { id = post.postId }, post);
        }

        // DELETE: api/posts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletepost(int id)
        {
       
            var post = await _context.posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            //deleting dependanceies
            List<postsLike> listOfLikes = _context.postsLikes.Where(o=>o.postId == post.postId).ToList();
            List<postsComment> listOfcomments = _context.postsComments.Where(o => o.postId == post.postId).ToList();
            List<postsShare> listOfshares = _context.postsShares.Where(o => o.postId == post.postId).ToList();

            foreach (postsShare share in listOfshares)
            {
                _context.postsShares.Remove(share);

            }
            foreach (postsLike like in listOfLikes)
            {
                _context.postsLikes.Remove(like);

            }
            foreach(postsComment comment in listOfcomments)
            {
                List<commentLike> commentLikes = _context.commentLikes.Where(like =>like.commentlikeId == comment.commentId).ToList();
                foreach (var commentLike in commentLikes)
                {
                    _context.commentLikes.Remove(commentLike);
                }
                _context.postsComments.Remove(comment);
            }


            //deleting post
            _context.posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        
        [HttpPost("byimage")]
        public async Task<ActionResult<post>> uploadImg(int id, string content,IFormFile? img)
        {
            user user;
            user = _context.users.FirstOrDefault(u => u.userId == id);
            if (_context.posts == null && img == null && user != null)
            {
                return Problem("Entity set 'fitnessgarageContext.posts'  is null.");
            }
            string imgName = DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(img.FileName);
            string path = "..//..//..//..//Coash DashBoard//coachDashboard//src//assets//images//test//" + imgName;
            using (var item = new FileStream(path, FileMode.Create))
            {
                img.CopyTo(item);
            }
            post post = new post();
            post.photoSrc = imgName;
            post.content = content;
            post.userid = user.userId;
            _context.posts.Add(post);
            await _context.SaveChangesAsync();
            return CreatedAtAction("Getpost", new { id = post.postId }, post);
        }


        [HttpGet("trending")]
        public async Task<ActionResult<IEnumerable<post>>> getTrending()
        {
            if (_context.posts == null)
            {
                return NotFound();
            }
            List<post> posts = await _context.posts.Include(c => c.postsComments).Include(a => a.user).ToListAsync();
            return posts;
        }

            private bool postExists(int id)
        {
            return (_context.posts?.Any(e => e.postId == id)).GetValueOrDefault();
        }
    }
}
