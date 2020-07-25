using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FerChat.Models {
    public class ChatMessage {
        public Guid Id { get; set; }

        public Guid ChatRoomId { get; set; }

        public Guid UserId { get; set; }

        public string TextContent { get; set; }

        [ForeignKey(nameof(ChatRoomId))]
        public ChatRoom ChatRoom { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
    }
}
