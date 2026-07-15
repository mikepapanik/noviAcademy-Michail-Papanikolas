using WorldRank.Domain.Player;

namespace WorldRank.Application.Interfaces;

public interface IPlayerRepository
{
    Task AddPlayerAsync(
        Player player,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Player>> GetAllPlayersAsync(
        CancellationToken cancellationToken);

    Task<Player?> FindPlayerAsync(
        int playerId,
        CancellationToken cancellationToken);

    Task UpdatePlayerAsync(
        Player player,
        CancellationToken cancellationToken);

    Task DeletePlayerAsync(
        int playerId,
        CancellationToken cancellationToken);
}