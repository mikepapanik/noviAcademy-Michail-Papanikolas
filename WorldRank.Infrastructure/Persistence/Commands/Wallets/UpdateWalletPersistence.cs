using WorldRank.Application.Infrastructure.Wallets;
using WorldRank.Application.Interfaces;
using WorldRank.Domain.Wallets;

namespace WorldRank.Infrastructure.Persistence.Commands.Wallets;

public sealed class UpdateWalletPersistence
    : IUpdateWalletPersistence
{
    private readonly IWalletRepository _walletRepository;

    public UpdateWalletPersistence(
        IWalletRepository walletRepository)
    {
        _walletRepository = walletRepository;
    }

    public async Task UpdateAsync(
        Wallet wallet,
        CancellationToken cancellationToken)
    {
        await _walletRepository.UpdateAsync(
            wallet,
            cancellationToken);
    }
}