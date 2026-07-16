using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Quartz;
using System.Text.Json.Serialization;
using WorldRank.Application;
using WorldRank.Application.Jobs;
using WorldRank.Gateway;
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

        // Application services, MediatR and strategies.
        services.AddApplication();

        // EF Core DbContext and database repositories.
        services.AddInfrastructure();

        // ECB typed HttpClient and resilience policies.
        services.AddGateway();

        // Quartz job for periodically fetching ECB rates.
        var currencyRatesJobKey = new JobKey(
            nameof(CurrencyRatesJob));

        services.AddQuartz(configuration =>
        {
            configuration.AddJob<CurrencyRatesJob>(
                options =>
                    options.WithIdentity(
                        currencyRatesJobKey));

            configuration.AddTrigger(options => options
                .ForJob(currencyRatesJobKey)
                .WithIdentity(
                    $"{nameof(CurrencyRatesJob)}-trigger")
                .StartAt(
                    DateTimeOffset.UtcNow.AddSeconds(10))
                .WithCronSchedule(
                    "0 0/1 * * * ?",
                    schedule => schedule
                        .WithMisfireHandlingInstructionDoNothing()));
        });

        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

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