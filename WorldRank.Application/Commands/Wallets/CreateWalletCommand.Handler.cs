using MediatR;
using WorldRank.Application.Infrastructure.Players;
using WorldRank.Application.Infrastructure.Wallets;
using WorldRank.Domain.Exceptions;
using WorldRank.Domain.Wallets;

namespace WorldRank.Application.Commands.Wallets;

public sealed class CreateWalletCommandHandler
    : IRequestHandler<CreateWalletCommand>
{
    private readonly IGetPlayerByIdPersistence _getPlayerByIdPersistence;
    private readonly ICreateWalletPersistence _createWalletPersistence;

    public CreateWalletCommandHandler(
        IGetPlayerByIdPersistence getPlayerByIdPersistence,
        ICreateWalletPersistence createWalletPersistence)
    {
        _getPlayerByIdPersistence = getPlayerByIdPersistence;
        _createWalletPersistence = createWalletPersistence;
    }

    public async Task Handle(
        CreateWalletCommand request,
        CancellationToken cancellationToken)
    {
        var player = await _getPlayerByIdPersistence.GetByIdAsync(
            request.PlayerId,
            cancellationToken);

        if (player is null)
        {
            throw new PlayerNotFoundException(
                request.PlayerId);
        }

        var wallet = new Wallet(
            request.Id,
            request.PlayerId,
            request.Currency,
            request.Balance,
            isBlocked: false);

        await _createWalletPersistence.CreateAsync(
            wallet,
            cancellationToken);
    }
}