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
                Guid userId = Request.Headers["User-Agent"].Any(x => x.Contains("Firefox"))
                    ? Guid.Parse("6a3614bc-1c99-4592-b04c-21c93abaf88b")
                    : Guid.Parse("1f5e245c-d373-4bf4-9dc6-649e3b6bd48e");
                var user = await _context.Users.FindAsync(userId);
                if (user == null) {
                    user = new Models.User {
                        Id = userId,
                        Name = $"User {userId}"
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