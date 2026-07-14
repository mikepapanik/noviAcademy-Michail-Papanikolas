using MediatR;
using WorldRank.Application.Infrastructure.Players;
using WorldRank.Domain.Player;

namespace WorldRank.Application.Queries.Players;

public sealed class GetPlayerByIdQueryHandler
    : IRequestHandler<GetPlayerByIdQuery, Player?>
{
    private readonly IGetPlayerByIdPersistence _getPlayerByIdPersistence;

    public GetPlayerByIdQueryHandler(
        IGetPlayerByIdPersistence getPlayerByIdPersistence)
    {
        _getPlayerByIdPersistence = getPlayerByIdPersistence;
    }

    public async Task<Player?> Handle(
        GetPlayerByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _getPlayerByIdPersistence.GetByIdAsync(
            request.Id,
            cancellationToken);
    }
}
