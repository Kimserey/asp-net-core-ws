using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreWebSockets.Controllers
{
    [Route("api/messages")]
    public class MessagesController : Controller
    {
        private IWebSocketService _service;

        public MessagesController(IWebSocketService service)
        {
            _service = service;
        }

        [HttpPost("{username}")]
        public IActionResult Post(string username, [FromBody] MessageViewModel message)
        {
            _service.Send(username, new Message { Content = message.Content, Date = DateTime.Now });
            return Ok(new { });
        }
    }
}
