using FerChat.Data;
using FerChat.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRChat.Hubs {
    public class ChatHub : Hub {
        private readonly FerChatDbContext _context;
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(FerChatDbContext context, ILogger<ChatHub> logger) {
            _context = context;
            _logger = logger;
        }

        public async Task JoinChatRoom(Guid chatRoomId) {
            Guid userId = Context.User.Claims
                .Where(x => x.Type == $"Room{chatRoomId}")
                .Select(x => x.Value)
                .Select(Guid.Parse)
                .Single();

            bool authorized = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => u.Name)
                .AnyAsync();
            if (!authorized)
                throw new Exception($"User is not logged into chat room {chatRoomId}");

            await Groups.AddToGroupAsync(Context.ConnectionId, $"ChatRoom:{chatRoomId}");
        }

        public async Task LeaveChatRoom(Guid chatRoomId) {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"ChatRoom:{chatRoomId}");
        }

        public async Task SendMessage(Guid chatRoomId, string message) {
            try {
                Guid userId = Context.User.Claims
                    .Where(x => x.Type == $"Room{chatRoomId}")
                    .Select(x => x.Value)
                    .Select(Guid.Parse)
                    .Single();

                string name = await _context.Users
                    .Where(u => u.Id == userId)
                    .Select(u => u.Name)
                    .SingleAsync();

                await Clients.Group($"ChatRoom:{chatRoomId}").SendAsync("ReceiveMessage", name, message);

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
                        UserId = userId,
                        TextContent = message,
                        Timestamp = DateTimeOffset.UtcNow
                    });
                    await _context.SaveChangesAsync();
                } catch (Exception ex) {
                    _logger.LogWarning(ex, "Could not save chat message to database");
                }
            } catch (Exception ex) {
                _logger.LogWarning(ex, "Could not send chat message via SignalR - discarding");
                return;
            }
        }
    }
}