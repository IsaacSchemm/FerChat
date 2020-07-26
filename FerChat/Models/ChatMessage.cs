using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FerChat.Models {
    public class ChatMessage {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string TextContent { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        [ForeignKey(nameof(UserId))]
        public ChatRoomParticipant User { get; set; }
    }
}
