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
    public class ChatMessagesController : ControllerBase {
        private readonly FerChatDbContext _context;

        public ChatMessagesController(FerChatDbContext context) {
            _context = context;
        }

        [HttpGet]
        [Route("api/rooms/{roomId}/messages")]
        public async Task<ActionResult<IEnumerable<ExistingChatMessage>>> GetChatRoomMessages(Guid roomId) {
            return await _context.ChatMessages
                .Include(p => p.User)
                .Where(p => p.User.ChatRoomId == roomId)
                .Select(p => new ExistingChatMessage {
                    Id = p.Id,
                    UserId = p.UserId,
                    TextContent = p.TextContent,
                    Timestamp = p.Timestamp
                })
                .ToListAsync();
        }

        [HttpGet]
        [Route("api/rooms/{roomId}/messages/{id}")]
        public async Task<ActionResult<ExistingChatMessage>> GetChatRoomMessage(Guid roomId, Guid id) {
            var chatRoomMessage = await _context.ChatMessages
                .Include(p => p.User)
                .Where(p => p.User.ChatRoomId == roomId)
                .Where(p => p.Id == id)
                .SingleOrDefaultAsync();

            if (chatRoomMessage == null) {
                return NotFound();
            }

            return new ExistingChatMessage {
                Id = chatRoomMessage.Id,
                UserId = chatRoomMessage.UserId,
                TextContent = chatRoomMessage.TextContent,
                Timestamp = chatRoomMessage.Timestamp
            };
        }

        [HttpDelete]
        [Route("api/rooms/{roomId}/messages/{id}")]
        public async Task<ActionResult> DeleteChatRoomMessage(Guid roomId, Guid id) {
            var chatRoomMessage = await _context.ChatMessages
                .Include(p => p.User)
                .Where(p => p.User.ChatRoomId == roomId)
                .Where(p => p.Id == id)
                .SingleOrDefaultAsync();

            if (chatRoomMessage == null) {
                return NotFound();
            }

            _context.ChatMessages.Remove(chatRoomMessage);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
