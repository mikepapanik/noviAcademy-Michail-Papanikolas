using WorldRank.Application.Infrastructure.Wallets;
using WorldRank.Application.Interfaces;
using WorldRank.Domain.Wallets;

namespace WorldRank.Infrastructure.Persistence.Queries.Wallets;

public sealed class GetWalletByIdPersistence
    : IGetWalletByIdPersistence
{
    private readonly IWalletRepository _walletRepository;

    public GetWalletByIdPersistence(
        IWalletRepository walletRepository)
    {
        _walletRepository = walletRepository;
    }

    public async Task<Wallet?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        return await _walletRepository.GetByIdAsync(
            id,
            cancellationToken);
    }
}