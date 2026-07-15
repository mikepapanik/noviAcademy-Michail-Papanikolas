using WorldRank.Application.Infrastructure.Players;
using WorldRank.Application.Interfaces;

namespace WorldRank.Infrastructure.Persistence.Commands.Players;

public sealed class DeletePlayerPersistence
    : IDeletePlayerPersistence
{
    private readonly IPlayerRepository _playerRepository;

    public DeletePlayerPersistence(
        IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public async Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var player = await _playerRepository.FindPlayerAsync(
            id,
            cancellationToken);

        if (player is null)
        {
            return false;
        }

        await _playerRepository.DeletePlayerAsync(
            id,
            cancellationToken);

        return true;
    }
}
