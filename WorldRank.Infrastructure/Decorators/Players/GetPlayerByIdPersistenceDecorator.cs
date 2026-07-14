using WorldRank.Application.Caching;
using WorldRank.Application.Infrastructure.Players;
using WorldRank.Domain.Player;

namespace WorldRank.Infrastructure.Decorators.Players;

public sealed class GetPlayerByIdPersistenceDecorator
    : IGetPlayerByIdPersistence
{
    private readonly IGetPlayerByIdPersistence _inner;
    private readonly ICache _cache;

    public GetPlayerByIdPersistenceDecorator(
        IGetPlayerByIdPersistence inner,
        ICache cache)
    {
        _inner = inner;
        _cache = cache;
    }

    public async Task<Player?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.PlayerById(id);

        var cachedPlayer = await _cache.GetAsync<Player>(
            cacheKey,
            cancellationToken);

        if (cachedPlayer is not null)
        {
            return cachedPlayer;
        }

        var player = await _inner.GetByIdAsync(
            id,
            cancellationToken);

        if (player is null)
        {
            return null;
        }

        await _cache.SetAsync(
            cacheKey,
            player,
            TimeSpan.FromMinutes(5),
            cancellationToken);

        return player;
    }
}