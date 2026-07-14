using WorldRank.Domain.Player;

namespace WorldRank.Application.Infrastructure.Players;

public interface IGetAllPlayersPersistence
{
    Task<IReadOnlyList<Player>> GetAllAsync(
        CancellationToken cancellationToken);
}
