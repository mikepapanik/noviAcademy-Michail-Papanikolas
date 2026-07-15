using MediatR;
using WorldRank.Application.Infrastructure.Players;
using WorldRank.Domain.Player;

namespace WorldRank.Application.Queries.Players;

public sealed class GetAllPlayersQueryHandler
    : IRequestHandler<GetAllPlayersQuery, IReadOnlyList<Player>>
{
    private readonly IGetAllPlayersPersistence _getAllPlayersPersistence;

    public GetAllPlayersQueryHandler(
        IGetAllPlayersPersistence getAllPlayersPersistence)
    {
        _getAllPlayersPersistence = getAllPlayersPersistence;
    }

    public async Task<IReadOnlyList<Player>> Handle(
        GetAllPlayersQuery request,
        CancellationToken cancellationToken)
    {
        return await _getAllPlayersPersistence.GetAllAsync(
            cancellationToken);
    }
}
