using WorldRank.Application.Caching;
using WorldRank.Application.Infrastructure.Wallets;
using WorldRank.Domain.Wallets;

namespace WorldRank.Infrastructure.Decorators.Wallets;

public sealed class CreateWalletPersistenceDecorator
    : ICreateWalletPersistence
{
    private readonly ICreateWalletPersistence _inner;
    private readonly ICache _cache;

    public CreateWalletPersistenceDecorator(
        ICreateWalletPersistence inner,
        ICache cache)
    {
        _inner = inner;
        _cache = cache;
    }

    public async Task CreateAsync(
        Wallet wallet,
        CancellationToken cancellationToken)
    {
        await _inner.CreateAsync(
            wallet,
            cancellationToken);

        await _cache.RemoveAsync(
            CacheKeys.WalletById(wallet.Id),
            cancellationToken);

        await _cache.RemoveAsync(
            CacheKeys.WalletsByPlayerId(wallet.PlayerId),
            cancellationToken);
    }
}
