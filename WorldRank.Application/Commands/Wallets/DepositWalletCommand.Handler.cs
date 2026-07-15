using MediatR;
using WorldRank.Application.Infrastructure.Wallets;
using WorldRank.Domain.Exceptions;
using WorldRank.Domain.Wallets;

namespace WorldRank.Application.Commands.Wallets;

public sealed class DepositWalletCommandHandler
    : IRequestHandler<DepositWalletCommand, Wallet>
{
    private readonly IGetWalletByIdPersistence _getWalletByIdPersistence;
    private readonly IUpdateWalletPersistence _updateWalletPersistence;

    public DepositWalletCommandHandler(
        IGetWalletByIdPersistence getWalletByIdPersistence,
        IUpdateWalletPersistence updateWalletPersistence)
    {
        _getWalletByIdPersistence = getWalletByIdPersistence;
        _updateWalletPersistence = updateWalletPersistence;
    }

    public async Task<Wallet> Handle(
        DepositWalletCommand request,
        CancellationToken cancellationToken)
    {
        var wallet = await _getWalletByIdPersistence.GetByIdAsync(
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