using Microsoft.EntityFrameworkCore;

namespace uwp
{
    public class MyDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=45.140.19.120;Database=nao_db;Username=postgres;Password=supersecretpassword");
        }
    }
}
