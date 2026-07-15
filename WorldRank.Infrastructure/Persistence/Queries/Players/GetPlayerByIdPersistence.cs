using WorldRank.Application.Infrastructure.Players;
using WorldRank.Application.Interfaces;
using WorldRank.Domain.Player;

namespace WorldRank.Infrastructure.Persistence.Queries.Players;

public sealed class GetPlayerByIdPersistence
    : IGetPlayerByIdPersistence
{
    private readonly IPlayerRepository _playerRepository;

    public GetPlayerByIdPersistence(
        IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public async Task<Player?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        return await _playerRepository.FindPlayerAsync(
            id,
            cancellationToken);
    }
}