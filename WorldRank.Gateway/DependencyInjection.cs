using Microsoft.Extensions.DependencyInjection;
using WorldRank.Application.ExchangeRates;
using WorldRank.Gateway.ExchangeRates;

namespace WorldRank.Gateway;

public static class DependencyInjection
{
    public static IServiceCollection AddGateway(
        this IServiceCollection services)
    {
        services
            .AddHttpClient<IEcbHttpClient, EcbHttpClient>()
            .AddStandardResilienceHandler(options =>
            {
                options.Retry.MaxRetryAttempts = 3;

                options.Retry.Delay =
                    TimeSpan.FromSeconds(2);

                options.Retry.BackoffType =
                    Polly.DelayBackoffType.Exponential;

                options.Retry.UseJitter = false;

                options.CircuitBreaker.FailureRatio = 1.0;

                options.CircuitBreaker.MinimumThroughput = 5;

                options.CircuitBreaker.SamplingDuration =
                    TimeSpan.FromSeconds(30);

                options.CircuitBreaker.BreakDuration =
                    TimeSpan.FromSeconds(30);

                options.AttemptTimeout.Timeout =
                    TimeSpan.FromSeconds(10);

                options.TotalRequestTimeout.Timeout =
                    TimeSpan.FromSeconds(60);
            });

        return services;
    }
}
