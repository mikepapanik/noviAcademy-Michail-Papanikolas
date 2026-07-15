using WorldRank.Application.Infrastructure.Players;
using WorldRank.Application.Interfaces;
using WorldRank.Domain.Player;

namespace WorldRank.Infrastructure.Persistence.Queries.Players;

public sealed class GetAllPlayersPersistence
    : IGetAllPlayersPersistence
{
    private readonly IPlayerRepository _playerRepository;

    public GetAllPlayersPersistence(
        IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public async Task<IReadOnlyList<Player>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await _playerRepository.GetAllPlayersAsync(
            cancellationToken);
    }
}
