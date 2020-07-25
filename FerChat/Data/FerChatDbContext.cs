using FerChat.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FerChat.Data {
    public class FerChatDbContext : DbContext {
        public DbSet<User> Users { get; set; }
        public DbSet<ChatRoom> ChatRooms { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

        public FerChatDbContext(DbContextOptions<FerChatDbContext> options) : base(options) { }
    }
}
