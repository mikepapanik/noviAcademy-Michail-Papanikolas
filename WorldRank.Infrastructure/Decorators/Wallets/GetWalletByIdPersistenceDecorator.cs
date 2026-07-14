using WorldRank.Application.Caching;
using WorldRank.Application.Infrastructure.Wallets;
using WorldRank.Domain.Wallets;

namespace WorldRank.Infrastructure.Decorators.Wallets;

public sealed class GetWalletByIdPersistenceDecorator
    : IGetWalletByIdPersistence
{
    private readonly IGetWalletByIdPersistence _inner;
    private readonly ICache _cache;

    public GetWalletByIdPersistenceDecorator(
        IGetWalletByIdPersistence inner,
        ICache cache)
    {
        _inner = inner;
        _cache = cache;
    }

    public async Task<Wallet?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.WalletById(id);

        var cachedWallet = await _cache.GetAsync<Wallet>(
            cacheKey,
            cancellationToken);

        if (cachedWallet is not null)
        {
            return cachedWallet;
        }

        var wallet = await _inner.GetByIdAsync(
            id,
            cancellationToken);

        if (wallet is null)
        {
            return null;
        }

        await _cache.SetAsync(
            cacheKey,
            wallet,
            TimeSpan.FromMinutes(5),
            cancellationToken);

        return wallet;
    }
}
