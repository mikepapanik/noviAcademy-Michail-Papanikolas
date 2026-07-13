using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using WorldRank.Application.Interfaces;
using WorldRank.Domain.Player;

namespace WorldRank.Application.Services;

public class PlayerService
{
    private const string AllPlayersCacheKey =
        "players:all";

    private static readonly TimeSpan CacheDuration =
        TimeSpan.FromMinutes(5);

    private readonly IPlayerRepository _playerRepository;
    private readonly IMemoryCache _cache;
    private readonly ILogger<PlayerService> _logger;

    public PlayerService(
        IPlayerRepository playerRepository,
        IMemoryCache cache,
        ILogger<PlayerService> logger)
    {
        _playerRepository = playerRepository;
        _cache = cache;
        _logger = logger;
    }

    public void AddPlayer(Player player)
    {
        _playerRepository.AddPlayer(player);

        InvalidatePlayerCache(player.Id);

        _logger.LogInformation(
            "Player {PlayerId} ({PlayerName}) added with score {Score}",
            player.Id,
            player.Name,
            player.Score);
    }

    public IEnumerable<Player> GetAllPlayers()
    {
        if (_cache.TryGetValue(
            AllPlayersCacheKey,
            out List<Player>? cachedPlayers))
        {
            _logger.LogInformation(
                "Players loaded from cache");

            return cachedPlayers!;
        }

        var players = _playerRepository
            .GetAllPlayers()
            .ToList();

        _cache.Set(
            AllPlayersCacheKey,
            players,
            CacheDuration);

        _logger.LogInformation(
            "Players loaded from repository and stored in cache");

        return players;
    }

    public Player? FindPlayer(int playerId)
    {
        var cacheKey = GetPlayerCacheKey(playerId);

        if (_cache.TryGetValue(
            cacheKey,
            out Player? cachedPlayer))
        {
            _logger.LogInformation(
                "Player {PlayerId} loaded from cache",
                playerId);

            return cachedPlayer;
        }

        var player = _playerRepository.FindPlayer(playerId);

        _cache.Set(
            cacheKey,
            player,
            CacheDuration);

        _logger.LogInformation(
            "Player {PlayerId} loaded from repository and stored in cache",
            playerId);

        return player;
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

        InvalidatePlayerCache(playerId);

        _logger.LogInformation(
            "Player {PlayerId} deleted",
            playerId);
    }

    public IEnumerable<IGrouping<int, Player>>
        GroupPlayersByScore()
    {
        return GetAllPlayers()
            .GroupBy(player => player.Score)
            .OrderByDescending(group => group.Key);
    }

    private void InvalidatePlayerCache(int playerId)
    {
        _cache.Remove(AllPlayersCacheKey);
        _cache.Remove(GetPlayerCacheKey(playerId));

        _logger.LogInformation(
            "Cache invalidated for player {PlayerId}",
            playerId);
    }

    private static string GetPlayerCacheKey(
        int playerId)
    {
        return $"players:{playerId}";
    }
}