using WorldRank.Domain.Wallets;

namespace WorldRank.Application.Infrastructure.Wallets;

public interface IUpdateWalletPersistence
{
    Task<Wallet?> GetByIdAsync(
        int walletId,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        Wallet wallet,
        CancellationToken cancellationToken);
}
