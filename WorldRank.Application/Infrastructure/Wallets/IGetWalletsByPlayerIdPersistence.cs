using WorldRank.Domain.Wallets;

namespace WorldRank.Application.Infrastructure.Wallets;

public interface IGetWalletsByPlayerIdPersistence
{
    Task<IReadOnlyList<Wallet>> GetByPlayerIdAsync(
        int playerId,
        CancellationToken cancellationToken);
}
