using System;

namespace FerChat.Api.Contracts {
    public class ChatRoomParticipant {
        public string Name { get; set; }
    }

    public class ExistingChatRoomParticipant : ChatRoomParticipant {
        public Guid Id { get; set; }
        public Guid ChatRoomId { get; set; }
    }
}
