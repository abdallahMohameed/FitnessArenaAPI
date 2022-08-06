using FitnessArena_API.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PusherServer;

namespace FitnessArena_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> HelloWorld(MessageDTO messageDTO)
        {
            var options = new PusherOptions
            {
                Cluster = "eu",
                Encrypted = true
            };

            var pusher = new Pusher(
              "1457632",
              "fefe358f37e6e32f25b2",
              "1b78a1c04c485b20de23",
              options);

             await pusher.TriggerAsync(
              "chat",
              "message",
              new {
                  message = messageDTO.message,username=messageDTO.username ,senddate=DateTime.Now,imgurl=messageDTO.imgurl
              });

            return Ok(new string[] {});
        }
    }
}
