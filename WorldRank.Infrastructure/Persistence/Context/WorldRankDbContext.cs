using Microsoft.EntityFrameworkCore;
using WorldRank.Domain.CurrencyRates;
using WorldRank.Domain.Player;
using WorldRank.Domain.Wallets;

namespace WorldRank.Infrastructure.Persistence.Context;

public class WorldRankDbContext : DbContext
{
    public DbSet<Player> Players => Set<Player>();

    public DbSet<Wallet> Wallets => Set<Wallet>();

    public DbSet<CurrencyRates> CurrencyRates =>
        Set<CurrencyRates>();

    public WorldRankDbContext(
        DbContextOptions<WorldRankDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Player>(entity =>
        {
            entity.ToTable("Players");

            entity.HasKey(player => player.Id);

            entity.Property(player => player.Id)
                .ValueGeneratedNever();

            entity.Property(player => player.Name)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(player => player.Score)
                .IsRequired();
        });

        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.ToTable("Wallets");

            entity.HasKey(wallet => wallet.Id);

            entity.Property(wallet => wallet.Id)
                .ValueGeneratedNever();

            entity.Property(wallet => wallet.PlayerId)
                .IsRequired();

            entity.Property(wallet => wallet.Currency)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(wallet => wallet.Balance)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(wallet => wallet.IsBlocked)
                .IsRequired();

            entity.HasOne<Player>()
                .WithMany()
                .HasForeignKey(wallet => wallet.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(wallet => wallet.PlayerId);

            entity.HasIndex(wallet => new
            {
                wallet.PlayerId,
                wallet.Currency
            })
            .IsUnique();
        });

        modelBuilder.Entity<CurrencyRates>(entity =>
        {
            entity.ToTable("CurrencyRates");

            entity.HasKey(currencyRate => new
            {
                currencyRate.Currency,
                currencyRate.Date
            });

            entity.Property(currencyRate =>
                    currencyRate.Currency)
                .HasMaxLength(3)
                .IsRequired();

            entity.Property(currencyRate =>
                    currencyRate.Rate)
                .HasPrecision(18, 8)
                .IsRequired();

            entity.Property(currencyRate =>
                    currencyRate.Date)
                .HasColumnType("date")
                .IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}