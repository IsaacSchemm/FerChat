using System;

namespace FerChat.Api.Contracts {
    public class ChatRoom {
        public string Name { get; set; }
    }

    public class ExistingChatRoom : ChatRoom {
        public Guid Id { get; set; }
    }
}
