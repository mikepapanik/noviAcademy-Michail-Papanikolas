using MediatR;
using WorldRank.Application.Infrastructure.Wallets;
using WorldRank.Domain.Exceptions;
using WorldRank.Domain.Wallets;

namespace WorldRank.Application.Commands.Wallets;

public sealed class DepositWalletCommandHandler
    : IRequestHandler<DepositWalletCommand, Wallet>
{
    private readonly IUpdateWalletPersistence _updateWalletPersistence;

    public DepositWalletCommandHandler(
        IUpdateWalletPersistence updateWalletPersistence)
    {
        _updateWalletPersistence = updateWalletPersistence;
    }

    public async Task<Wallet> Handle(
        DepositWalletCommand request,
        CancellationToken cancellationToken)
    {
        var wallet = await _updateWalletPersistence.GetByIdAsync(
            request.WalletId,
            cancellationToken);

        if (wallet is null)
        {
            throw new WalletNotFoundException(request.WalletId);
        }

        wallet.Deposit(request.Amount);

        await _updateWalletPersistence.UpdateAsync(
            wallet,
            cancellationToken);

        return wallet;
    }
}
