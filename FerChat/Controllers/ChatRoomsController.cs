using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FerChat.Data;
using FerChat.Models;

namespace FerChat.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    [BasicAuthentication]
    public class ChatRoomsController : ControllerBase {
        private readonly FerChatDbContext _context;

        public ChatRoomsController(FerChatDbContext context) {
            _context = context;
        }

        // GET: api/ChatRooms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChatRoom>>> GetChatRooms() {
            return await _context.ChatRooms.ToListAsync();
        }

        // GET: api/ChatRooms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ChatRoom>> GetChatRoom(Guid id) {
            var chatRoom = await _context.ChatRooms.FindAsync(id);

            if (chatRoom == null) {
                return NotFound();
            }

            return chatRoom;
        }

        // PUT: api/ChatRooms/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutChatRoom(Guid id, ChatRoom chatRoom) {
            if (id != chatRoom.Id) {
                return BadRequest();
            }

            _context.Entry(chatRoom).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!ChatRoomExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ChatRooms
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<ChatRoom>> PostChatRoom(ChatRoom chatRoom) {
            _context.ChatRooms.Add(chatRoom);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetChatRoom", new { id = chatRoom.Id }, chatRoom);
        }

        // DELETE: api/ChatRooms/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ChatRoom>> DeleteChatRoom(Guid id) {
            var chatRoom = await _context.ChatRooms.FindAsync(id);
            if (chatRoom == null) {
                return NotFound();
            }

            _context.ChatRooms.Remove(chatRoom);
            await _context.SaveChangesAsync();

            return chatRoom;
        }

        private bool ChatRoomExists(Guid id) {
            return _context.ChatRooms.Any(e => e.Id == id);
        }
    }
}
