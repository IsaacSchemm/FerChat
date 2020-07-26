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
    public class ChatRoomParticipantsController : ControllerBase {
        private readonly FerChatDbContext _context;

        public ChatRoomParticipantsController(FerChatDbContext context) {
            _context = context;
        }

        [HttpGet]
        [Route("api/rooms/{roomId}/participants")]
        public async Task<ActionResult<IEnumerable<ChatRoomParticipant>>> GetChatRoomParticipants(Guid roomId) {
            return await _context.ChatRoomParticipants
                .Where(p => p.ChatRoomId == roomId)
                .ToListAsync();
        }

        [HttpGet]
        [Route("api/rooms/{roomId}/participants/{id}")]
        public async Task<ActionResult<ChatRoomParticipant>> GetChatRoomParticipant(Guid roomId, Guid id) {
            var chatRoomParticipant = await _context.ChatRoomParticipants
                .Where(p => p.ChatRoomId == roomId)
                .Where(p => p.Id == id)
                .SingleOrDefaultAsync();

            if (chatRoomParticipant == null) {
                return NotFound();
            }

            return chatRoomParticipant;
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
        public async Task<ActionResult<ChatRoomParticipant>> PostChatRoomParticipant(Guid roomId, ChatRoomParticipant chatRoomParticipant) {
            chatRoomParticipant.ChatRoomId = roomId;
            _context.ChatRoomParticipants.Add(chatRoomParticipant);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetChatRoomParticipant", new { roomId, id = chatRoomParticipant.Id }, chatRoomParticipant);
        }

        [HttpDelete]
        [Route("api/rooms/{roomId}/participants/{id}")]
        public async Task<ActionResult<ChatRoomParticipant>> DeleteChatRoomParticipant(Guid roomId, Guid id) {
            var chatRoomParticipant = await _context.ChatRoomParticipants
                .Where(p => p.ChatRoomId == roomId)
                .Where(p => p.Id == id)
                .SingleOrDefaultAsync();

            if (chatRoomParticipant == null) {
                return NotFound();
            }

            _context.ChatRoomParticipants.Remove(chatRoomParticipant);
            await _context.SaveChangesAsync();

            return chatRoomParticipant;
        }
    }
}
