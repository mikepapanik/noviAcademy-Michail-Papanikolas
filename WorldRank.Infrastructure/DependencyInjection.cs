using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WorldRank.Application.Interfaces;
using WorldRank.Infrastructure.Persistence.Context;
using WorldRank.Infrastructure.Repositories;

namespace WorldRank.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services)
    {
        services.AddDbContext<WorldRankDbContext>(options =>
            options.UseSqlServer(
                "Server=localhost;" +
                "Database=WorldRank;" +
                "Integrated Security=true;" +
                "TrustServerCertificate=true"));

        services.AddScoped<
            IPlayerRepository,
            DBPlayerRepository>();

        services.AddScoped<
            IWalletRepository,
            DBWalletRepository>();

        services.AddScoped<
            ICurrencyRatesRepository,
            DBCurrencyRatesRepository>();

        return services;
    }
}