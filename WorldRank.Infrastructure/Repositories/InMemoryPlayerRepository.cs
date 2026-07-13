using WorldRank.Application.Interfaces;
using WorldRank.Domain.Player;

namespace WorldRank.Infrastructure.Repositories
{
	public class InMemoryPlayerRepository : IPlayerRepository
	{

		private List<Player> _players;

		public InMemoryPlayerRepository()
		{
			_players = new List<Player>();
		}

		public void AddPlayer(Player player)
		{
			_players.Add(player);
		}

		public IEnumerable<Player> GetAllPlayers()
		{
			// Return a copy so callers cannot mutate the repository's internal list.
			return _players.ToList();
		}

		public void DeletePlayer(int playerId)
		{
			var player = _players.Where(item => item.Id == playerId).FirstOrDefault();

			if (player is null)
			{
				return;
			}

			_players.Remove(player);
		}

		public Player? FindPlayer(int playerId)
		{
			return _players.Where(item => item.Id == playerId).FirstOrDefault();
		}

		public IEnumerable<IGrouping<int, Player>> GroupPlayersByScore()
		{
			return _players
				.GroupBy(player => player.Score)
				.OrderByDescending(group => group.Key);
		}
	}
}
