using WorldRank.Application.Infrastructure.Players;
using WorldRank.Application.Interfaces;
using WorldRank.Domain.Player;

namespace WorldRank.Infrastructure.Persistence.Commands.Players;

public sealed class UpdatePlayerPersistence
    : IUpdatePlayerPersistence
{
    private readonly IPlayerRepository _playerRepository;

    public UpdatePlayerPersistence(
        IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public async Task<bool> UpdateAsync(
        Player player,
        CancellationToken cancellationToken)
    {
        var existingPlayer =
            await _playerRepository.FindPlayerAsync(
                player.Id,
                cancellationToken);

        if (existingPlayer is null)
        {
            return false;
        }

        await _playerRepository.UpdatePlayerAsync(
            player,
            cancellationToken);

        return true;
    }
}
