using WorldRank.Application.Infrastructure.Wallets;
using WorldRank.Application.Interfaces;
using WorldRank.Domain.Wallets;

namespace WorldRank.Infrastructure.Persistence.Queries.Wallets;

public sealed class GetWalletsByPlayerIdPersistence
    : IGetWalletsByPlayerIdPersistence
{
    private readonly IWalletRepository _walletRepository;

    public GetWalletsByPlayerIdPersistence(
        IWalletRepository walletRepository)
    {
        _walletRepository = walletRepository;
    }

    public async Task<IReadOnlyList<Wallet>> GetByPlayerIdAsync(
        int playerId,
        CancellationToken cancellationToken)
    {
        return await _walletRepository
            .GetAllWalletsByPlayerIdAsync(
                playerId,
                cancellationToken);
    }
}