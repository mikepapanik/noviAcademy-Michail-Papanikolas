using WorldRank.Domain.Wallets;

namespace WorldRank.Application.Infrastructure.Wallets;

public interface ICreateWalletPersistence
{
    Task CreateAsync(
        Wallet wallet,
        CancellationToken cancellationToken);
}
