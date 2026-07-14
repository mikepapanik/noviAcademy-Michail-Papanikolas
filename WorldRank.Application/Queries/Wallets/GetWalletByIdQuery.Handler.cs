using MediatR;
using WorldRank.Application.Infrastructure.Wallets;
using WorldRank.Domain.Wallets;

namespace WorldRank.Application.Queries.Wallets;

public sealed class GetWalletByIdQueryHandler
    : IRequestHandler<GetWalletByIdQuery, Wallet?>
{
    private readonly IGetWalletByIdPersistence _getWalletByIdPersistence;

    public GetWalletByIdQueryHandler(
        IGetWalletByIdPersistence getWalletByIdPersistence)
    {
        _getWalletByIdPersistence = getWalletByIdPersistence;
    }

    public async Task<Wallet?> Handle(
        GetWalletByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _getWalletByIdPersistence.GetByIdAsync(
            request.Id,
            cancellationToken);
    }
}
