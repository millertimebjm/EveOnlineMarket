using Eve.Models.EveApi;
using Eve.Models;
using Eve.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using Eve.Configurations;
using Microsoft.Extensions.Logging;

namespace Eve.Repositories.Context;

public class EveDbContext : DbContext
{
    public EveOnlineMarketConfigurationService _configuration;

    public EveDbContext(
        DbContextOptions<EveDbContext> options,
        IOptionsSnapshot<EveOnlineMarketConfigurationService> optionsSnapshot
        )
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
            .LogTo(Console.WriteLine, LogLevel.Information);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Changing Database table name to Metadata
        modelBuilder
            .Entity<Order>()
            .ToTable("MarketOrder")
            .HasKey(mo => mo.OrderId);
        modelBuilder
            .Entity<EveType>()
            .ToTable("Type")
            .HasKey(t => t.TypeId);
        modelBuilder
            .Entity<User>()
            .ToTable("User")
            .HasKey(u => u.UserId);
        modelBuilder
            .Entity<Planet>()
            .ToTable("Planet")
            .HasKey(p => p.PlanetId);
    }

    public DbSet<Order> MarketOrders { get; set; }
    public DbSet<EveType> Types { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Planet> Planets { get; set; }
}