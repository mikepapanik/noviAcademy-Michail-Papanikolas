using WorldRank.Domain.Wallets;

namespace WorldRank.Application.Infrastructure.Wallets;

public interface IUpdateWalletPersistence
{
    Task UpdateAsync(
        Wallet wallet,
        CancellationToken cancellationToken);
}
