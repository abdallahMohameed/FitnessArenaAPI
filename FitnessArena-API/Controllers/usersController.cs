using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessArena_API.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FitnessArena_API.Controllers { 

    //comment
    [Route("api/[controller]")]
    [ApiController]
    public class usersController : ControllerBase
    {
        private readonly fitnessgarageContext _context;

        public usersController(fitnessgarageContext context)
        {
            _context = context;
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<user>>> Getusers()
        {
            if (_context.users == null)
            {
                return NotFound();
            }
            return await _context.users.ToListAsync();
        }

        // GET: api/users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<user>> Getuser(int id)
        {
            if (_context.users == null)
            {
                return NotFound();
            }
            var user = await _context.users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }
        [HttpGet("coaches")]
        public async Task<ActionResult<IEnumerable<user>>> GetChoaches()
        {
            if (_context.users == null)
            {
                return NotFound();
            }
            List<user> coaches = await _context.users
                .Where(s => s.type == "coach")
                .ToListAsync();
                
            return coaches;
        }

        // PUT: api/users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Putuser(int id, user user)
        {
            user oldData = _context.users.FirstOrDefault( i => i.userId == id);
            if (oldData == null && user == null)
            {
                return BadRequest();
            }
            else
            {
                oldData.birthDate = user.birthDate;
                oldData.fname = user.fname;
                oldData.lname = user.lname;
                oldData.CategoryName = user.CategoryName;
                oldData.address = user.address;
            }
            try
            {
                await _context.SaveChangesAsync();
                return Ok(oldData);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!userExists(id))
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

        // POST: api/users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("signup")]
        public async Task<ActionResult<user>> Postuser(user newUser)
        {
            var tok = "";
            if (ModelState.IsValid)
            {
                string cryptPass = encryptPass(newUser.password);
                newUser.password = cryptPass;
                newUser.PhotoSrc = newUser.gender+".jpg";
                _context.users.Add(newUser);
                await _context.SaveChangesAsync();
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("thesecretkeyisfitness_Garage"));
                var data = new List<Claim>();
                data.Add(new Claim("userId", newUser.userId.ToString()));
                data.Add(new Claim("type", newUser.type));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(

                claims: data,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

                tok = new JwtSecurityTokenHandler().WriteToken(token);
            }
            if (_context.users == null)
            {
                return Problem("Entity set 'fitnessgarageContext.users'  is null.");
            }
            return CreatedAtAction("Getuser", new { id = newUser.userId }, new { data = newUser, theToken = tok });
        }

        // DELETE: api/users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deleteuser(int id)
        {
            if (_context.users == null)
            {
                return NotFound();
            }
            var user = await _context.users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool userExists(int id)
        {
            return (_context.users?.Any(e => e.userId == id)).GetValueOrDefault();
        }
        //login
        [HttpPost("login")]
        public ActionResult login(login userdata)
        {
            if (ModelState.IsValid)
            {
                user user;
                string cryptPass = encryptPass(userdata.password);
                user = _context.users.FirstOrDefault(
                    u => u.email == userdata.email && u.password == cryptPass);
                if (user == null)
                {
                    return Unauthorized();
                }
                else
                {
                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("thesecretkeyisfitness_Garage"));
                    var data = new List<Claim>();
                    data.Add(new Claim("userId", user.userId.ToString()));
                    data.Add(new Claim("type", user.type));
                    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(

                    claims: data,
                    expires: DateTime.Now.AddMinutes(120),
                    signingCredentials: credentials);

                    var tok = new JwtSecurityTokenHandler().WriteToken(token);
                    var json = new {
                        theToken = tok,
                        userType = user.type,
                        userId = user.userId.ToString(),
                    };
                    return Ok(json);
                }
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpPost("image/{id}")]
        public ActionResult uploadImg(int id, IFormFile img)
        {
            user user;
            user = _context.users.FirstOrDefault(u => u.userId == id);
            string imgName = DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(img.FileName);
            string path = "..//..//..//..//Coash DashBoard//coachDashboard//src//assets//images//test//" + imgName;
            using (var item = new FileStream(path, FileMode.Create))
            {
                img.CopyTo(item);
            }
            user.PhotoSrc = imgName;
             _context.SaveChanges();
            return Ok(user);
        }
        [HttpPost("changepassword/{id}")]
        public async Task<ActionResult> changePass(int id, changePass change )
        {
            string cryptOld = encryptPass(change.oldPass);
            user user;
            user = _context.users.FirstOrDefault(
                   u => u.userId == id && u.password == cryptOld);
            if (user != null && change.newPass != null)
            {
                string cryptNew = encryptPass(change.newPass);
                user.password = cryptNew;
                await _context.SaveChangesAsync();
                return Ok(user);
            }
            return BadRequest();

        }
        public static string encryptPass(string pass)
        {
            string secrateKey = "FitnessGarage";
            byte[] toArr = new byte[pass.Length + secrateKey.Length];
            toArr = System.Text.Encoding.UTF8.GetBytes(pass + secrateKey);
            string cryptPass = Convert.ToBase64String(toArr);
            return cryptPass;
        }

        //list trainee ==> coach bundel ==>take coach id

        //public async Task<ActionResult<IEnumerable<user>>> GetChoacheClient(int coachID)
        //{
        //    user? coach = _context.users.Include(a=>a.traineeBundles).FirstOrDefault(a=>a.userId == coachID);

        //     if (coach != null && coach.type=="coach")
        //    {
        //        List<user> clients = _context.bundles.Include(a=>a.traineeBundles).
        //    }
         
        //    List<user> coaches = await _context.users
        //        .Where(s => s.type == "coach")
        //        .ToListAsync();

        //    return coaches;
        //}

        [HttpGet("clients/{id}")]
        public async Task<ActionResult<IEnumerable<user>>> GetClinte(int id)
        {
            var bundleIds = await _context.bundles
                .Where(i => i.coachId == id)
                .Select(x => x.bundleId)
                .ToListAsync();

            var traineeIds = await _context.traineeBundles
                .Where(b => bundleIds.Contains(b.bundleId))
                .Select ( y => y.traineeId)
                .ToListAsync();

            List<user> clients = await _context.users
                .Where(c => traineeIds.Contains(c.userId))
                .ToListAsync();

            return Ok(clients);
        }
    }
    
}
