using WorldRank.Application.Caching;
using WorldRank.Application.Infrastructure.Players;
using WorldRank.Domain.Player;

namespace WorldRank.Infrastructure.Decorators.Players;

public sealed class CreatePlayerPersistenceDecorator
    : ICreatePlayerPersistence
{
    private readonly ICreatePlayerPersistence _inner;
    private readonly ICache _cache;

    public CreatePlayerPersistenceDecorator(
        ICreatePlayerPersistence inner,
        ICache cache)
    {
        _inner = inner;
        _cache = cache;
    }

    public async Task CreateAsync(
        Player player,
        CancellationToken cancellationToken)
    {
        await _inner.CreateAsync(
            player,
            cancellationToken);

        await _cache.RemoveAsync(
            CacheKeys.AllPlayers,
            cancellationToken);

        await _cache.SetAsync(
            CacheKeys.PlayerById(player.Id),
            player,
            TimeSpan.FromMinutes(5),
            cancellationToken);
    }
}