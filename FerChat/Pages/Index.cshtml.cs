using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FerChat.Data;
using FerChat.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FerChat.Pages {
    public class IndexModel : PageModel {
        private readonly FerChatDbContext _context;
        private readonly ILogger<IndexModel> _logger;

        public Guid ChatRoomId { get; set; }
        public IEnumerable<ChatMessage> ChatMessages { get; set; } = Enumerable.Empty<ChatMessage>();

        public IndexModel(FerChatDbContext context, ILogger<IndexModel> logger) {
            _context = context;
            _logger = logger;
        }

        public async Task OnGetAsync(Guid chatRoomId) {
            try {
                Guid userId = User.Claims
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

                ChatRoomId = chatRoomId;
                ChatMessages = await _context.ChatMessages
                    .Include(m => m.User)
                    .Where(m => m.User.ChatRoomId == chatRoomId)
                    .OrderBy(m => m.Timestamp)
                    .ToListAsync();
            } catch (Exception ex) {
                _logger.LogWarning(ex, $"Could not load existing chat messages for chat room {chatRoomId}");
            }
        }
    }
}
