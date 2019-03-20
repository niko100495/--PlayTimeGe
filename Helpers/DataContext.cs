using WebApi.Entities;
using Microsoft.EntityFrameworkCore;


namespace WebApi.Helpers
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Roles> Role { get; set; }
        public DbSet<Console> Consoles { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Magazine> Magazines { get; set; }
        public DbSet<StaffHistory> StaffHistories { get; set; }
        public DbSet<PromotionConsole> PromotionConsoles { get; set; }
        public DbSet<Trade> Trades { get; set; }
        public DbSet<ConsolePlay> ConsolePlays { get; set; }
        
       
    }
}