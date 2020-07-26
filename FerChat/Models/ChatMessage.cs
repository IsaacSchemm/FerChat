using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FerChat.Models {
    public class ChatMessage {
        public Guid Id { get; set; }

        public Guid ChatRoomId { get; set; }

        [Required]
        public string UserId { get; set; }

        public string TextContent { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        [ForeignKey(nameof(ChatRoomId))]
        public ChatRoom ChatRoom { get; set; }

        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; }
    }
}
