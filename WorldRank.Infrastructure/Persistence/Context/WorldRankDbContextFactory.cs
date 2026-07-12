using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WorldRank.Infrastructure.Persistence.Context;

public class WorldRankDbContextFactory
    : IDesignTimeDbContextFactory<WorldRankDbContext>
{
    public WorldRankDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder =
            new DbContextOptionsBuilder<WorldRankDbContext>();

        optionsBuilder.UseSqlServer(
            "Server=localhost;" +
            "Database=WorldRank;" +
            "Integrated Security=true;" +
            "TrustServerCertificate=true");

        return new WorldRankDbContext(
            optionsBuilder.Options);
    }
}
