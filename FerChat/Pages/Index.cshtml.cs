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

        public IEnumerable<ChatMessage> ChatMessages { get; set; }

        public IndexModel(FerChatDbContext context, ILogger<IndexModel> logger) {
            _context = context;
            _logger = logger;
        }

        public async Task OnGetAsync() {
            ChatMessages = await _context.ChatMessages
                .Include(m => m.User)
                .ToListAsync();
        }
    }
}
