using Autofac;
using WorldRank.Application.Infrastructure.Players;
using WorldRank.Infrastructure.Decorators.Players;
using WorldRank.Application.Infrastructure.Wallets;
using WorldRank.Infrastructure.Decorators.Wallets;
namespace WorldRank.Infrastructure;

public sealed class InfrastructureModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var infrastructureAssembly =
            typeof(InfrastructureModule).Assembly;

        builder
            .RegisterAssemblyTypes(infrastructureAssembly)
            .Where(type => type.Name.EndsWith("Persistence"))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        builder.RegisterDecorator<
            CreatePlayerPersistenceDecorator,
            ICreatePlayerPersistence>();

        builder.RegisterDecorator<
            UpdatePlayerPersistenceDecorator,
            IUpdatePlayerPersistence>();

        builder.RegisterDecorator<
            DeletePlayerPersistenceDecorator,
            IDeletePlayerPersistence>();

        builder.RegisterDecorator<
            GetPlayerByIdPersistenceDecorator,
            IGetPlayerByIdPersistence>();

        builder.RegisterDecorator<
            GetAllPlayersPersistenceDecorator,
            IGetAllPlayersPersistence>();
        builder.RegisterDecorator<
            CreateWalletPersistenceDecorator,
            ICreateWalletPersistence>();
        builder.RegisterDecorator<
            UpdateWalletPersistenceDecorator,
            IUpdateWalletPersistence>();
        builder.RegisterDecorator<
            GetWalletByIdPersistenceDecorator,
            IGetWalletByIdPersistence>();
        builder.RegisterDecorator<
            GetWalletsByPlayerIdPersistenceDecorator,
            IGetWalletsByPlayerIdPersistence>();
    }
}