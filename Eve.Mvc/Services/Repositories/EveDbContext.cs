using Eve.Mvc.Models;
using EveOnlineMarket.Eve.Mvc.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;

namespace Eve.Mvc.Services.Repositories
{
    public class EveDbContext : DbContext
    {
        public EveOnlineMarketConfigurationService _configuration;

        public EveDbContext(
            DbContextOptions<EveDbContext> options,
            IOptionsSnapshot<EveOnlineMarketConfigurationService> optionsSnapshot)
            : base(options)
        {
            _configuration = optionsSnapshot.Value;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            Console.WriteLine(_configuration.GetConnectionString());
            optionsBuilder
                .UseNpgsql(_configuration.GetConnectionString())
                .EnableSensitiveDataLogging()
                .LogTo(Console.WriteLine, LogLevel.Information); ;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Changing Database table name to Metadata
            modelBuilder
                .Entity<EveMarketOrder>()
                .ToTable("MarketOrder")
                .HasKey(mo => mo.OrderId);
            modelBuilder
                .Entity<EveUniverseType>()
                .ToTable("Type")
                .HasKey(t => t.TypeId);
            modelBuilder
                .Entity<User>()
                .ToTable("User")
                .HasKey(u => u.UserId);
        }

        public DbSet<EveMarketOrder> MarketOrders { get; set; }
        public DbSet<EveUniverseType> Types { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
