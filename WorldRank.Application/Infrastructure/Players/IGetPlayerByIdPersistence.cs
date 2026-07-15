using WorldRank.Domain.Player;

namespace WorldRank.Application.Infrastructure.Players;

public interface IGetPlayerByIdPersistence
{
    Task<Player?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);
}
