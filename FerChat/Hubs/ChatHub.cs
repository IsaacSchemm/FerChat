using FerChat.Data;
using FerChat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace SignalRChat.Hubs {
    [Authorize]
    public class ChatHub : Hub {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(ApplicationDbContext context, UserManager<IdentityUser> userManager, ILogger<ChatHub> logger) {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task JoinChatRoom(Guid chatRoomId) {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"ChatRoom:{chatRoomId}");
        }

        public async Task LeaveChatRoom(Guid chatRoomId) {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"ChatRoom:{chatRoomId}");
        }

        public async Task SendMessage(Guid chatRoomId, string user, string message) {
            try {
                await Clients.Group($"ChatRoom:{chatRoomId}").SendAsync("ReceiveMessage", user, message);
            } catch (Exception ex) {
                _logger.LogWarning(ex, "Could not send chat message via SignalR - discarding");
                return;
            }

            try {
                var chatRoom = await _context.ChatRooms.FindAsync(chatRoomId);

                if (chatRoom == null) {
                    chatRoom = new ChatRoom {
                        Id = chatRoomId,
                        Name = "Chat Room #1"
                    };
                }

                _context.ChatMessages.Add(new ChatMessage {
                    Id = Guid.NewGuid(),
                    ChatRoom = chatRoom,
                    UserId = _userManager.GetUserId(Context.User),
                    TextContent = message,
                    Timestamp = DateTimeOffset.UtcNow
                });
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                _logger.LogWarning(ex, "Could not save chat message to database");
            }
        }
    }
}