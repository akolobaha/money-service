using Microsoft.EntityFrameworkCore;
using MoneyService.Model;

namespace MoneyService.DB
{
    public class ApplicationContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }

        /*public ApplicationContext()
        {
            Database.EnsureCreated();
        }*/
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Database=test;Username=postgres;Password=515986");
        }
    }
}
