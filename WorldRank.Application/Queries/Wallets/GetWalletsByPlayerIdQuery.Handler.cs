using MediatR;
using WorldRank.Application.Infrastructure.Wallets;
using WorldRank.Domain.Wallets;

namespace WorldRank.Application.Queries.Wallets;

public sealed class GetWalletsByPlayerIdQueryHandler
    : IRequestHandler<
        GetWalletsByPlayerIdQuery,
        IReadOnlyList<Wallet>>
{
    private readonly IGetWalletsByPlayerIdPersistence
        _getWalletsByPlayerIdPersistence;

    public GetWalletsByPlayerIdQueryHandler(
        IGetWalletsByPlayerIdPersistence
            getWalletsByPlayerIdPersistence)
    {
        _getWalletsByPlayerIdPersistence =
            getWalletsByPlayerIdPersistence;
    }

    public async Task<IReadOnlyList<Wallet>> Handle(
        GetWalletsByPlayerIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _getWalletsByPlayerIdPersistence
            .GetByPlayerIdAsync(
                request.PlayerId,
                cancellationToken);
    }
}