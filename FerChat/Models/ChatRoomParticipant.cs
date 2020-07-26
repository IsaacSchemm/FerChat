using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FerChat.Models {
    public class ChatRoomParticipant {
        public Guid Id { get; set; }

        public Guid ChatRoomId { get; set; }

        [Required]
        public string Name { get; set; }

        [ForeignKey(nameof(ChatRoomId))]
        public ChatRoom ChatRoom { get; set; }
    }
}
