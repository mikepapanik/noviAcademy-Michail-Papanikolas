using WorldRank.Domain.Player;

namespace WorldRank.Application.Infrastructure.Players;

public interface ICreatePlayerPersistence
{
    Task CreateAsync(
        Player player,
        CancellationToken cancellationToken);
}
