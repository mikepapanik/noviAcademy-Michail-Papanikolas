using Microsoft.Extensions.DependencyInjection;
using WorldRank.Application.Services;
using WorldRank.Application.Strategies;

namespace WorldRank.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IFundsStrategy, AddFundsStrategy>();
        services.AddSingleton<IFundsStrategy, SubtractFundsStrategy>();
        services.AddSingleton<IFundsStrategy, ForceSubtractFundsStrategy>();

        services.AddSingleton<PlayerService>();
        services.AddSingleton<WalletService>();

        return services;
    }
}