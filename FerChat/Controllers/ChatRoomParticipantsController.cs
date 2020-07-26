using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FerChat.Data;
using FerChat.Models;

namespace FerChat.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [BasicAuthentication]
    public class ChatRoomParticipantsController : ControllerBase {
        private readonly FerChatDbContext _context;

        public ChatRoomParticipantsController(FerChatDbContext context) {
            _context = context;
        }

        // GET: api/ChatRoomParticipants
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChatRoomParticipant>>> GetChatRoomParticipants() {
            return await _context.ChatRoomParticipants.ToListAsync();
        }

        // GET: api/ChatRoomParticipants/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ChatRoomParticipant>> GetChatRoomParticipant(Guid id) {
            var chatRoomParticipant = await _context.ChatRoomParticipants.FindAsync(id);

            if (chatRoomParticipant == null) {
                return NotFound();
            }

            return chatRoomParticipant;
        }

        // PUT: api/ChatRoomParticipants/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutChatRoomParticipant(Guid id, ChatRoomParticipant chatRoomParticipant) {
            if (id != chatRoomParticipant.Id) {
                return BadRequest();
            }

            _context.Entry(chatRoomParticipant).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!ChatRoomParticipantExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ChatRoomParticipants
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<ChatRoomParticipant>> PostChatRoomParticipant(ChatRoomParticipant chatRoomParticipant) {
            _context.ChatRoomParticipants.Add(chatRoomParticipant);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetChatRoomParticipant", new { id = chatRoomParticipant.Id }, chatRoomParticipant);
        }

        // DELETE: api/ChatRoomParticipants/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ChatRoomParticipant>> DeleteChatRoomParticipant(Guid id) {
            var chatRoomParticipant = await _context.ChatRoomParticipants.FindAsync(id);
            if (chatRoomParticipant == null) {
                return NotFound();
            }

            _context.ChatRoomParticipants.Remove(chatRoomParticipant);
            await _context.SaveChangesAsync();

            return chatRoomParticipant;
        }

        private bool ChatRoomParticipantExists(Guid id) {
            return _context.ChatRoomParticipants.Any(e => e.Id == id);
        }
    }
}
