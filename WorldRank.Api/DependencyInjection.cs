using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System.Text.Json.Serialization;
using WorldRank.Application;
using WorldRank.Infrastructure;

namespace WorldRank.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(
        this IServiceCollection services,
        ILoggingBuilder logging)
    {
        // NLog configuration.
        logging.ClearProviders();
        logging.AddNLog("nlog.config");

        // Application services and strategies.
        services.AddApplication();

        // EF Core DbContext and database repositories.
        services.AddInfrastructure();

        // In-memory cache.
        services.AddMemoryCache();

        // Controllers and enum serialization.
        services
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options
                    .JsonSerializerOptions
                    .Converters
                    .Add(new JsonStringEnumConverter());
            });

        // Swagger / OpenAPI.
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }
}