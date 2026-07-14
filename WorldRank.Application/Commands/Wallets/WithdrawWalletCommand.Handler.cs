using MediatR;
using WorldRank.Application.Infrastructure.Wallets;
using WorldRank.Domain.Exceptions;

namespace WorldRank.Application.Commands.Wallets;

public sealed class WithdrawWalletCommandHandler
    : IRequestHandler<WithdrawWalletCommand>
{
    private readonly IUpdateWalletPersistence _updateWalletPersistence;

    public WithdrawWalletCommandHandler(
        IUpdateWalletPersistence updateWalletPersistence)
    {
        _updateWalletPersistence = updateWalletPersistence;
    }

    public async Task Handle(
        WithdrawWalletCommand request,
        CancellationToken cancellationToken)
    {
        var wallet = await _updateWalletPersistence.GetByIdAsync(
            request.WalletId,
            cancellationToken);

        if (wallet is null)
        {
            throw new WalletNotFoundException(request.WalletId);
        }

        wallet.Withdraw(request.Amount);

        await _updateWalletPersistence.UpdateAsync(
            wallet,
            cancellationToken);
    }
}
