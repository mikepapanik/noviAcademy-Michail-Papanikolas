using WorldRank.Application.Caching;
using WorldRank.Application.Infrastructure.Wallets;
using WorldRank.Domain.Wallets;

namespace WorldRank.Infrastructure.Decorators.Wallets;

public sealed class UpdateWalletPersistenceDecorator
    : IUpdateWalletPersistence
{
    private readonly IUpdateWalletPersistence _inner;
    private readonly ICache _cache;

    public UpdateWalletPersistenceDecorator(
        IUpdateWalletPersistence inner,
        ICache cache)
    {
        _inner = inner;
        _cache = cache;
    }

    public Task<Wallet?> GetByIdAsync(
        int walletId,
        CancellationToken cancellationToken)
    {
        return _inner.GetByIdAsync(
            walletId,
            cancellationToken);
    }

    public async Task UpdateAsync(
        Wallet wallet,
        CancellationToken cancellationToken)
    {
        await _inner.UpdateAsync(
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
