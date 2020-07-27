using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FerChat.Data;
using FerChat.Api.Contracts;

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
        public async Task<ActionResult<IEnumerable<ExistingChatRoom>>> GetChatRooms() {
            return await _context.ChatRooms
                .Select(r => new ExistingChatRoom {
                    Id = r.Id,
                    Name = r.Name
                }).ToListAsync();
        }

        [HttpGet]
        [Route("api/rooms/{id}")]
        public async Task<ActionResult<ExistingChatRoom>> GetChatRoom(Guid id) {
            var chatRoom = await _context.ChatRooms.FindAsync(id);

            if (chatRoom == null) {
                return NotFound();
            }

            return new ExistingChatRoom {
                Id = chatRoom.Id,
                Name = chatRoom.Name
            };
        }

        [HttpPut]
        [Route("api/rooms/{id}")]
        public async Task<IActionResult> PutChatRoom(Guid id, ChatRoom newProperties) {
            var chatRoom = await _context.ChatRooms.FindAsync(id);

            if (chatRoom == null) {
                return NotFound();
            }

            chatRoom.Name = newProperties.Name;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost]
        [Route("api/rooms")]
        public async Task<ActionResult<ExistingChatRoom>> PostChatRoom(ChatRoom newProperties) {
            var chatRoom = new Models.ChatRoom {
                Name = newProperties.Name
            };
            _context.ChatRooms.Add(chatRoom);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetChatRoom", new { id = chatRoom.Id }, chatRoom);
        }

        [HttpDelete]
        [Route("api/rooms/{id}")]
        public async Task<ActionResult> DeleteChatRoom(Guid id) {
            var chatRoom = await _context.ChatRooms.FindAsync(id);
            if (chatRoom == null) {
                return NotFound();
            }

            _context.ChatRooms.Remove(chatRoom);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
