using WorldRank.Application.Caching;
using WorldRank.Application.Infrastructure.Players;
using WorldRank.Domain.Player;

namespace WorldRank.Infrastructure.Decorators.Players;

public sealed class GetAllPlayersPersistenceDecorator
    : IGetAllPlayersPersistence
{
    private readonly IGetAllPlayersPersistence _inner;
    private readonly ICache _cache;

    public GetAllPlayersPersistenceDecorator(
        IGetAllPlayersPersistence inner,
        ICache cache)
    {
        _inner = inner;
        _cache = cache;
    }

    public async Task<IReadOnlyList<Player>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var cachedPlayers =
            await _cache.GetAsync<IReadOnlyList<Player>>(
                CacheKeys.AllPlayers,
                cancellationToken);

        if (cachedPlayers is not null)
        {
            return cachedPlayers;
        }

        var players = await _inner.GetAllAsync(
            cancellationToken);

        await _cache.SetAsync(
            CacheKeys.AllPlayers,
            players,
            TimeSpan.FromMinutes(5),
            cancellationToken);

        return players;
    }
}
