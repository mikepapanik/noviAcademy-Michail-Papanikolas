using Autofac;
using WorldRank.Application.Caching;
using WorldRank.Infrastructure.Caching;

namespace WorldRank.Api;

public sealed class ApiModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder
            .RegisterType<MemoryCacheStore>()
            .As<ICache>()
            .SingleInstance();
    }
}
