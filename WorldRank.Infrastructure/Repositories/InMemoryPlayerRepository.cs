using WorldRank.Application.Interfaces;
using WorldRank.Domain.Player;

namespace WorldRank.Infrastructure.Repositories
{
    public class InMemoryPlayerRepository : IPlayerRepository
    {

        private readonly List<Player> _players = [];

        public InMemoryPlayerRepository()
        {
            _players = new List<Player>();
        }

        public Task AddPlayerAsync(Player player, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _players.Add(player);

            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<Player>> GetAllPlayersAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            IReadOnlyList<Player> players =
                _players.ToList();

            return Task.FromResult(players);
        }

        public Task<Player?> FindPlayerAsync(int playerId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var player = _players.FirstOrDefault(item => item.Id == playerId);

            return Task.FromResult(player);
        }

        public Task DeletePlayerAsync(int playerId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var player = _players.FirstOrDefault(
                item => item.Id == playerId);

            if (player is not null)
            {
                _players.Remove(player);
            }

            return Task.CompletedTask;
        }
    }
}