using Microsoft.EntityFrameworkCore;
using MessengerBackend.Models;

namespace MessengerBackend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }


}
