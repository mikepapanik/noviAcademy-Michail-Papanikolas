using Microsoft.Extensions.Logging;
using WorldRank.Application.Interfaces;
using WorldRank.Domain.Player;

namespace WorldRank.Application.Services;

public class PlayerService
{
    private readonly IPlayerRepository _playerRepository;
    private readonly ILogger<PlayerService> _logger;

    public PlayerService(
        IPlayerRepository playerRepository,
        ILogger<PlayerService> logger)
    {
        _playerRepository = playerRepository;
        _logger = logger;
    }

    public void AddPlayer(Player player)
    {
        _playerRepository.AddPlayer(player);

        _logger.LogInformation(
            "Player {PlayerId} ({PlayerName}) added with score {Score}",
            player.Id,
            player.Name,
            player.Score);
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
        var player = _playerRepository.FindPlayer(playerId);

        if (player is null)
        {
            _logger.LogWarning(
                "Delete skipped because player {PlayerId} was not found",
                playerId);

            return;
        }

        _playerRepository.DeletePlayer(playerId);

        _logger.LogInformation(
            "Player {PlayerId} deleted",
            playerId);
    }

    public IEnumerable<IGrouping<int, Player>> GroupPlayersByScore()
    {
        return _playerRepository.GroupPlayersByScore();
    }
}