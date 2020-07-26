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
    [BasicAuthentication]
    public class ChatRoomsController : ControllerBase {
        private readonly FerChatDbContext _context;

        public ChatRoomsController(FerChatDbContext context) {
            _context = context;
        }

        [HttpGet]
        [Route("api/rooms")]
        public async Task<ActionResult<IEnumerable<ChatRoom>>> GetChatRooms() {
            return await _context.ChatRooms.ToListAsync();
        }

        [HttpGet]
        [Route("api/rooms/{id}")]
        public async Task<ActionResult<ChatRoom>> GetChatRoom(Guid id) {
            var chatRoom = await _context.ChatRooms.FindAsync(id);

            if (chatRoom == null) {
                return NotFound();
            }

            return chatRoom;
        }

        [HttpPut]
        [Route("api/rooms/{id}")]
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

        [HttpPost]
        [Route("api/rooms")]
        public async Task<ActionResult<ChatRoom>> PostChatRoom(ChatRoom chatRoom) {
            _context.ChatRooms.Add(chatRoom);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetChatRoom", new { id = chatRoom.Id }, chatRoom);
        }

        [HttpDelete]
        [Route("api/rooms/{id}")]
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
