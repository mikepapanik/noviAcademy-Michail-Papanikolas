using MediatR;
using Microsoft.Extensions.DependencyInjection;
using WorldRank.Application.Behaviors;
using WorldRank.Application.Strategies;

namespace WorldRank.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(
                typeof(DependencyInjection).Assembly);
        });

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(LoggingBehavior<,>));

        services.AddScoped<IFundsStrategy, AddFundsStrategy>();
        services.AddScoped<IFundsStrategy, SubtractFundsStrategy>();
        services.AddScoped<IFundsStrategy, ForceSubtractFundsStrategy>();

        return services;
    }
}