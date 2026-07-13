using Microsoft.EntityFrameworkCore;
using _user;
namespace _appDbContext
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    }
}