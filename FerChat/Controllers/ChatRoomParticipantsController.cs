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
    public class ChatRoomParticipantsController : ControllerBase {
        private readonly FerChatDbContext _context;

        public ChatRoomParticipantsController(FerChatDbContext context) {
            _context = context;
        }

        [HttpGet]
        [Route("api/rooms/{roomId}/participants")]
        public async Task<ActionResult<IEnumerable<ExistingChatRoomParticipant>>> GetChatRoomParticipants(Guid roomId) {
            return await _context.ChatRoomParticipants
                .Where(p => p.ChatRoomId == roomId)
                .Select(p => new ExistingChatRoomParticipant {
                    Id = p.Id,
                    ChatRoomId = p.ChatRoomId,
                    Name = p.Name
                })
                .ToListAsync();
        }

        [HttpGet]
        [Route("api/rooms/{roomId}/participants/{id}")]
        public async Task<ActionResult<ExistingChatRoomParticipant>> GetChatRoomParticipant(Guid roomId, Guid id) {
            var chatRoomParticipant = await _context.ChatRoomParticipants
                .Where(p => p.ChatRoomId == roomId)
                .Where(p => p.Id == id)
                .SingleOrDefaultAsync();

            if (chatRoomParticipant == null) {
                return NotFound();
            }

            return new ExistingChatRoomParticipant {
                Id = chatRoomParticipant.Id,
                ChatRoomId = chatRoomParticipant.ChatRoomId,
                Name = chatRoomParticipant.Name
            };
        }

        [HttpPut]
        [Route("api/rooms/{roomId}/participants/{id}")]
        public async Task<IActionResult> PutChatRoomParticipant(Guid roomId, Guid id, ChatRoomParticipant newProperties) {
            var chatRoomParticipant = await _context.ChatRoomParticipants
                .Where(p => p.ChatRoomId == roomId)
                .Where(p => p.Id == id)
                .SingleOrDefaultAsync();

            if (chatRoomParticipant == null) {
                return NotFound();
            }

            chatRoomParticipant.Name = newProperties.Name;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost]
        [Route("api/rooms/{roomId}/participants")]
        public async Task<ActionResult<ExistingChatRoomParticipant>> PostChatRoomParticipant(Guid roomId, ChatRoomParticipant newProperties) {
            var chatRoomParticipant = new Models.ChatRoomParticipant {
                ChatRoomId = roomId,
                Name = newProperties.Name
            };
            _context.ChatRoomParticipants.Add(chatRoomParticipant);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetChatRoomParticipant", new { roomId, id = chatRoomParticipant.Id }, chatRoomParticipant);
        }

        [HttpDelete]
        [Route("api/rooms/{roomId}/participants/{id}")]
        public async Task<ActionResult> DeleteChatRoomParticipant(Guid roomId, Guid id) {
            var chatRoomParticipant = await _context.ChatRoomParticipants
                .Where(p => p.ChatRoomId == roomId)
                .Where(p => p.Id == id)
                .SingleOrDefaultAsync();

            if (chatRoomParticipant == null) {
                return NotFound();
            }

            _context.ChatRoomParticipants.Remove(chatRoomParticipant);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
