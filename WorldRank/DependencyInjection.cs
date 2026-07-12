using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using WorldRank.Application;
using WorldRank.Infrastructure;

namespace WorldRank;

public static class DependencyInjection
{
    public static IServiceCollection AddWorldRank(this IServiceCollection services)
    {
        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.SetMinimumLevel(LogLevel.Trace);
            builder.AddNLog();
        });

        services.AddApplication();
        services.AddInfrastructure();

        return services;
    }
}