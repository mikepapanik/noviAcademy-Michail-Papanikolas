using WorldRank.Application.Caching;
using WorldRank.Application.Infrastructure.Players;

namespace WorldRank.Infrastructure.Decorators.Players;

public sealed class DeletePlayerPersistenceDecorator
    : IDeletePlayerPersistence
{
    private readonly IDeletePlayerPersistence _inner;
    private readonly ICache _cache;

    public DeletePlayerPersistenceDecorator(
        IDeletePlayerPersistence inner,
        ICache cache)
    {
        _inner = inner;
        _cache = cache;
    }

    public async Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var deleted = await _inner.DeleteAsync(
            id,
            cancellationToken);

        if (!deleted)
        {
            return false;
        }

        await _cache.RemoveAsync(
            CacheKeys.PlayerById(id),
            cancellationToken);

        await _cache.RemoveAsync(
            CacheKeys.AllPlayers,
            cancellationToken);

        return true;
    }
}
