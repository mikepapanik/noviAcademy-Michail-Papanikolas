using WorldRank.Domain.Player;

namespace WorldRank.Application.Infrastructure.Players;

public interface IUpdatePlayerPersistence
{
    Task<bool> UpdateAsync(
        Player player,
        CancellationToken cancellationToken);
}
