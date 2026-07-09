using WorldRank.Domain.Player;

namespace WorldRank.Infrastructure.Repositories
{
	public interface IPlayerRepository
	{
		void AddPlayer(Player player);

		IEnumerable<Player> GetAllPlayers();

		void DeletePlayer(int playerId);

		Player? FindPlayer(int playerId);

		IEnumerable<IGrouping<int, Player>> GroupPlayersByScore();
	}
}