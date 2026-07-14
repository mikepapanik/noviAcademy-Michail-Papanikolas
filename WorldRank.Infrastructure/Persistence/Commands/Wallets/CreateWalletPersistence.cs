using WorldRank.Application.Infrastructure.Wallets;
using WorldRank.Application.Interfaces;
using WorldRank.Domain.Wallets;

namespace WorldRank.Infrastructure.Persistence.Commands.Wallets;

public sealed class CreateWalletPersistence
    : ICreateWalletPersistence
{
    private readonly IWalletRepository _walletRepository;

    public CreateWalletPersistence(
        IWalletRepository walletRepository)
    {
        _walletRepository = walletRepository;
    }

    public async Task CreateAsync(
        Wallet wallet,
        CancellationToken cancellationToken)
    {
        await _walletRepository.AddAsync(
            wallet,
            cancellationToken);
    }
}