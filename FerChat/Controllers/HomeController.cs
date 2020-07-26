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
    public class HomeController : Controller {
        private readonly FerChatDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(FerChatDbContext context, ILogger<HomeController> logger) {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index() {
            return View();
        }

        public IActionResult Privacy() {
            return View();
        }

        public async Task<IActionResult> Login(Guid chatRoomId) {
            try {
                Guid userId = Guid.NewGuid();
                var user = await _context.Users.FindAsync(userId);
                if (user == null) {
                    var room = await _context.ChatRooms
                        .Where(r => r.Id == chatRoomId)
                        .SingleOrDefaultAsync();
                    if (room == null) {
                        room = new ChatRoom {
                            Id = chatRoomId,
                            Name = $"{chatRoomId}"
                        };
                    }

                    user = new ChatRoomParticipant {
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
                return RedirectToAction("Index", "Chat", new {
                    chatRoomId
                });
            } catch (Exception ex) {
                _logger.LogError(ex, "Could not log user in");
                return StatusCode(500);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
