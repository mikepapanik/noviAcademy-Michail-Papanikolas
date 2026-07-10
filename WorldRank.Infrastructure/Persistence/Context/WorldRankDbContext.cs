using Microsoft.EntityFrameworkCore;
using WorldRank.Domain.Player;
using WorldRank.Domain.Wallets;

namespace WorldRank.Infrastructure.Persistence.Context;

public class WorldRankDbContext : DbContext
{
    public DbSet<Player> Players { get; set; }
    public DbSet<Wallet> Wallets { get; set; }

    public WorldRankDbContext(
        DbContextOptions<WorldRankDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Player>(x =>
        {
            x.ToTable("Players");

            x.HasKey(p => p.Id);

            x.Property(p => p.Id)
                .ValueGeneratedNever();

            x.Property(p => p.Name)
                .HasMaxLength(100)
                .IsRequired();
        });

        modelBuilder.Entity<Wallet>(x =>
        {
            x.ToTable("Wallets");

            x.HasKey(w => w.Id);

            x.Property(w => w.Id)
                .ValueGeneratedNever();

            x.Property(w => w.PlayerId)
                .IsRequired();

            x.Property(w => w.Currency)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            x.Property(w => w.Balance)
                .HasPrecision(18, 2)
                .IsRequired();

            x.Property(w => w.IsBlocked)
                .IsRequired();

            x.HasIndex(w => w.PlayerId);
        });

        base.OnModelCreating(modelBuilder);
    }
}