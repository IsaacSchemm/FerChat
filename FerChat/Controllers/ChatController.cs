using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FerChat.Models;
using FerChat.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace FerChat.Controllers {
    public class ChatController : Controller {
        private readonly FerChatDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public ChatController(FerChatDbContext context, ILogger<HomeController> logger) {
            _context = context;
            _logger = logger;
        }

        public ActionResult Index() {
            return View();
        }

        public async Task<IActionResult> Messages(Guid chatRoomId) {
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

                return Ok(await _context.ChatMessages
                    .Include(m => m.User)
                    .Where(m => m.User.ChatRoomId == chatRoomId)
                    .OrderBy(m => m.Timestamp)
                    .Select(m => new {
                        m.Id,
                        m.TextContent,
                        m.Timestamp,
                        User = new {
                            m.User.Id,
                            m.User.Name
                        }
                    })
                    .ToListAsync());
            } catch (Exception ex) {
                _logger.LogWarning(ex, $"Could not load existing chat messages for chat room {chatRoomId}");
                return StatusCode(500);
            }
        }
    }
}
