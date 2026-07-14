using MediatR;
using WorldRank.Application.Infrastructure.Wallets;
using WorldRank.Domain.Exceptions;

namespace WorldRank.Application.Commands.Wallets;

public sealed class UnblockWalletCommandHandler
    : IRequestHandler<UnblockWalletCommand>
{
    private readonly IUpdateWalletPersistence _updateWalletPersistence;

    public UnblockWalletCommandHandler(
        IUpdateWalletPersistence updateWalletPersistence)
    {
        _updateWalletPersistence = updateWalletPersistence;
    }

    public async Task Handle(
        UnblockWalletCommand request,
        CancellationToken cancellationToken)
    {
        var wallet = await _updateWalletPersistence.GetByIdAsync(
            request.WalletId,
            cancellationToken);

        if (wallet is null)
        {
            throw new WalletNotFoundException(request.WalletId);
        }

        wallet.Unblock();

        await _updateWalletPersistence.UpdateAsync(
            wallet,
            cancellationToken);
    }
}
