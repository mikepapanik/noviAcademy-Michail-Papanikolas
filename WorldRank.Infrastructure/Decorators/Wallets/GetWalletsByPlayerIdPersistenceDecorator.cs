using WorldRank.Application.Caching;
using WorldRank.Application.Infrastructure.Wallets;
using WorldRank.Domain.Wallets;

namespace WorldRank.Infrastructure.Decorators.Wallets;

public sealed class GetWalletsByPlayerIdPersistenceDecorator
    : IGetWalletsByPlayerIdPersistence
{
    private readonly IGetWalletsByPlayerIdPersistence _inner;
    private readonly ICache _cache;

    public GetWalletsByPlayerIdPersistenceDecorator(
        IGetWalletsByPlayerIdPersistence inner,
        ICache cache)
    {
        _inner = inner;
        _cache = cache;
    }

    public async Task<IReadOnlyList<Wallet>> GetByPlayerIdAsync(
        int playerId,
        CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.WalletsByPlayerId(playerId);

        var cachedWallets =
            await _cache.GetAsync<IReadOnlyList<Wallet>>(
                cacheKey,
                cancellationToken);

        if (cachedWallets is not null)
        {
            return cachedWallets;
        }

        var wallets = await _inner.GetByPlayerIdAsync(
            playerId,
            cancellationToken);

        await _cache.SetAsync(
            cacheKey,
            wallets,
            TimeSpan.FromMinutes(5),
            cancellationToken);

        return wallets;
    }
}
