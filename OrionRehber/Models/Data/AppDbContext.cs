using Microsoft.EntityFrameworkCore;
using OrionRehber.Models;

namespace OrionRehber.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Kullanici> Kullanici { get; set; }
        public DbSet<Rehber> Rehber { get; set; }
    }
}
