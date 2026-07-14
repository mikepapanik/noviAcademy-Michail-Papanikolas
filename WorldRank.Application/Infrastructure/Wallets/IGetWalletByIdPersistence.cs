using WorldRank.Domain.Wallets;

namespace WorldRank.Application.Infrastructure.Wallets;

public interface IGetWalletByIdPersistence
{
    Task<Wallet?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);
}
