using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChattingManagement.Service.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entity;
using Microsoft.AspNetCore.SignalR;

namespace ChattingManagement.Controller
{
    [Route("api/message")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly RallyWaveContext _context;
        private readonly IHubContext<ChatHub> _hubContext;

        public MessageController(IHubContext<ChatHub> hubContext, RallyWaveContext context)
        {
            _hubContext = hubContext;
            _context = context;
        }
        
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage(int conservationId, int senderId, string senderName, string content)
        {
            var conservation =
                await _context.Conservations.FirstOrDefaultAsync(c => c.ConservationId == conservationId);
            if (conservation != null)
            {
                var sender = await _context.Users.FirstOrDefaultAsync(u => u.UserId == senderId);
                if (sender != null)
                {
                    var message = new Message()
                    {
                        Conservation = conservation,
                        Status = 1,
                        Sender = senderId,
                        DateTime = DateTime.Now,
                        Content = content,
                        ConservationId = conservation.ConservationId,
                        SenderNavigation = sender
                    };
                    // add message to conservation
                    conservation.Messages.Add(message);
                    // save
                    _context.Conservations.Update(conservation);
                    await _context.SaveChangesAsync();
                    // publish message
                    await _hubContext.Clients.Group(conservationId.ToString()).SendAsync("ReceiveMessage", conservationId, senderId, senderName, content);
                    return Ok(new { Message = "Message sent successfully" });
                }
                return BadRequest("Sender not found!");
            }
            return BadRequest("Conservation not found!");
        }
        
        // GET: api/Message
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessages()
        {
            return await _context.Messages.ToListAsync();
        }

        // GET: api/Message/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Message>> GetMessage(int id)
        {
            var message = await _context.Messages.FindAsync(id);

            if (message == null)
            {
                return NotFound();
            }

            return message;
        }

        // PUT: api/Message/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMessage(int id, Message message)
        {
            if (id != message.MessageId)
            {
                return BadRequest();
            }

            _context.Entry(message).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MessageExists(id))
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

        // POST: api/Message
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Message>> PostMessage(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMessage", new { id = message.MessageId }, message);
        }

        // DELETE: api/Message/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MessageExists(int id)
        {
            return _context.Messages.Any(e => e.MessageId == id);
        }
    }
}
