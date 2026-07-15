using MediatR;
using WorldRank.Application.Infrastructure.Wallets;
using WorldRank.Domain.Exceptions;

namespace WorldRank.Application.Commands.Wallets;

public sealed class BlockWalletCommandHandler
    : IRequestHandler<BlockWalletCommand>
{
    private readonly IGetWalletByIdPersistence _getWalletByIdPersistence;
    private readonly IUpdateWalletPersistence _updateWalletPersistence;

    public BlockWalletCommandHandler(
        IGetWalletByIdPersistence getWalletByIdPersistence,
        IUpdateWalletPersistence updateWalletPersistence)
    {
        _getWalletByIdPersistence = getWalletByIdPersistence;
        _updateWalletPersistence = updateWalletPersistence;
    }

    public async Task Handle(
        BlockWalletCommand request,
        CancellationToken cancellationToken)
    {
        var wallet = await _getWalletByIdPersistence.GetByIdAsync(
            request.WalletId,
            cancellationToken);

        if (wallet is null)
        {
            throw new WalletNotFoundException(request.WalletId);
        }

        wallet.Block();

        await _updateWalletPersistence.UpdateAsync(
            wallet,
            cancellationToken);
    }
}