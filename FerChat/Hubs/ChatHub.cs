using FerChat.Data;
using FerChat.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace SignalRChat.Hubs {
    public class ChatHub : Hub {
        private readonly FerChatDbContext _context;

        public ChatHub(FerChatDbContext context) {
            _context = context;
        }

        public async Task SendMessage(string user, string message) {
            await Clients.All.SendAsync("ReceiveMessage", user, message);

            Guid chatRoomId = Guid.Parse("3c59801f-617d-4329-9973-00b686e4b4ad");
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
                User = new User {
                    Id = Guid.NewGuid(),
                    Name = user
                },
                TextContent = message
            });
            await _context.SaveChangesAsync();
        }
    }
}