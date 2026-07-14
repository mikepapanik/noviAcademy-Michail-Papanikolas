using WorldRank.Application.Caching;
using WorldRank.Application.Infrastructure.Players;
using WorldRank.Domain.Player;

namespace WorldRank.Infrastructure.Decorators.Players;

public sealed class UpdatePlayerPersistenceDecorator
    : IUpdatePlayerPersistence
{
    private readonly IUpdatePlayerPersistence _inner;
    private readonly ICache _cache;

    public UpdatePlayerPersistenceDecorator(
        IUpdatePlayerPersistence inner,
        ICache cache)
    {
        _inner = inner;
        _cache = cache;
    }

    public async Task<bool> UpdateAsync(
        Player player,
        CancellationToken cancellationToken)
    {
        var updated = await _inner.UpdateAsync(
            player,
            cancellationToken);

        if (!updated)
        {
            return false;
        }

        await _cache.RemoveAsync(
            CacheKeys.PlayerById(player.Id),
            cancellationToken);

        await _cache.RemoveAsync(
            CacheKeys.AllPlayers,
            cancellationToken);

        return true;
    }
}
