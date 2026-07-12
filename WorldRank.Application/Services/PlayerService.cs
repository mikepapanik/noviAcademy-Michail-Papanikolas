using WorldRank.Application.Interfaces;
using WorldRank.Domain.Player;

namespace WorldRank.Application.Services;

public class PlayerService
{
    private readonly IPlayerRepository _playerRepository;

    public PlayerService(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public void AddPlayer(Player player)
    {
        _playerRepository.AddPlayer(player);
    }

    public IEnumerable<Player> GetAllPlayers()
    {
        return _playerRepository.GetAllPlayers();
    }

    public Player? FindPlayer(int playerId)
    {
        return _playerRepository.FindPlayer(playerId);
    }

    public void DeletePlayer(int playerId)
    {
        _playerRepository.DeletePlayer(playerId);
    }

    public IEnumerable<IGrouping<int, Player>> GroupPlayersByScore()
    {
        return _playerRepository.GroupPlayersByScore();
    }
}