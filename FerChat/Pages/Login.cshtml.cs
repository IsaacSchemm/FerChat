using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FerChat.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FerChat.Pages {
    public class LoginModel : PageModel {
        private readonly FerChatDbContext _context;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(FerChatDbContext context, ILogger<LoginModel> logger) {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(Guid chatRoomId) {
            try {
                Guid userId = Guid.NewGuid();
                var user = await _context.Users.FindAsync(userId);
                if (user == null) {
                    var room = await _context.ChatRooms
                        .Where(r => r.Id == chatRoomId)
                        .SingleOrDefaultAsync();
                    if (room == null) {
                        room = new Models.ChatRoom {
                            Id = chatRoomId,
                            Name = $"{chatRoomId}"
                        };
                    }

                    user = new Models.ChatRoomParticipant {
                        Id = userId,
                        ChatRoom = room,
                        Name = Request.Headers["User-Agent"]
                    };
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }
                if (user == null)
                    throw new Exception($"User with ID {userId} not found");

                var claims = User.Claims
                    .Where(x => x.Type != $"Room{chatRoomId}")
                    .Concat(new[] {
                        new Claim($"Room{chatRoomId}", userId.ToString())
                    })
                    .ToList();
                var claimsIdentity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));
                return Redirect($"/?chatRoomId={chatRoomId}");
            } catch (Exception ex) {
                _logger.LogError(ex, "Could not log user in");
                return Page();
            }
        }
    }
}