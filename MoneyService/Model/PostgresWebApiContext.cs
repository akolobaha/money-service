using Microsoft.EntityFrameworkCore;

namespace MoneyService.Model
{
    public class PostgresWebApiContext : DbContext
    {
        public PostgresWebApiContext(DbContextOptions<PostgresWebApiContext> options) : base(options) { }
        public DbSet<UserModel> UserModel { get; set; }
    }
}