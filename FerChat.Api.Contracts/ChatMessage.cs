using System;

namespace FerChat.Api.Contracts {
    public class ExistingChatMessage {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string TextContent { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}
