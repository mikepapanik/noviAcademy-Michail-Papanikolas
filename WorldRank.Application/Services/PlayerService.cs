using Microsoft.Extensions.Logging;
using WorldRank.Application.Caching;
using WorldRank.Application.Interfaces;
using WorldRank.Domain.Player;

namespace WorldRank.Application.Services;

public class PlayerService
{
    private const string AllPlayersCacheKey =
        "players:all";

    private static readonly TimeSpan CacheDuration =
        TimeSpan.FromSeconds(60);

    private readonly IPlayerRepository _playerRepository;
    private readonly ICache _cache;
    private readonly ILogger<PlayerService> _logger;

    public PlayerService(
        IPlayerRepository playerRepository,
        ICache cache,
        ILogger<PlayerService> logger)
    {
        _playerRepository = playerRepository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Player> AddPlayerAsync(
        Player player,
        CancellationToken cancellationToken)
    {
        await _playerRepository.AddPlayerAsync(
            player,
            cancellationToken);

        await _cache.SetAsync(
            GetPlayerCacheKey(player.Id),
            player,
            CacheDuration,
            cancellationToken);

        await _cache.RemoveAsync(
            AllPlayersCacheKey,
            cancellationToken);

        _logger.LogInformation(
            "Player {PlayerId} ({PlayerName}) added with score {Score}",
            player.Id,
            player.Name,
            player.Score);

        return player;
    }

    public async Task<IReadOnlyList<Player>> GetAllPlayersAsync(
        CancellationToken cancellationToken)
    {
        var cachedPlayers =
            await _cache.GetAsync<IReadOnlyList<Player>>(
                AllPlayersCacheKey,
                cancellationToken);

        if (cachedPlayers is not null)
        {
            _logger.LogInformation(
                "Players loaded from cache");

            return cachedPlayers;
        }

        var players = await _playerRepository
            .GetAllPlayersAsync(
                cancellationToken);

        await _cache.SetAsync(
            AllPlayersCacheKey,
            players,
            CacheDuration,
            cancellationToken);

        _logger.LogInformation(
            "Players loaded from repository and stored in cache");

        return players;
    }

    public async Task<Player?> FindPlayerAsync(
        int playerId,
        CancellationToken cancellationToken)
    {
        var cacheKey =
            GetPlayerCacheKey(playerId);

        var cachedPlayer =
            await _cache.GetAsync<Player>(
                cacheKey,
                cancellationToken);

        if (cachedPlayer is not null)
        {
            _logger.LogInformation(
                "Player {PlayerId} loaded from cache",
                playerId);

            return cachedPlayer;
        }

        var player =
            await _playerRepository.FindPlayerAsync(
                playerId,
                cancellationToken);

        if (player is not null)
        {
            await _cache.SetAsync(
                cacheKey,
                player,
                CacheDuration,
                cancellationToken);

            _logger.LogInformation(
                "Player {PlayerId} loaded from repository and stored in cache",
                playerId);
        }

        return player;
    }

    public async Task DeletePlayerAsync(
        int playerId,
        CancellationToken cancellationToken)
    {
        var player =
            await _playerRepository.FindPlayerAsync(
                playerId,
                cancellationToken);

        if (player is null)
        {
            _logger.LogWarning(
                "Delete skipped because player {PlayerId} was not found",
                playerId);

            return;
        }

        await _playerRepository.DeletePlayerAsync(
            playerId,
            cancellationToken);

        await InvalidatePlayerCacheAsync(
            playerId,
            cancellationToken);

        _logger.LogInformation(
            "Player {PlayerId} deleted",
            playerId);
    }

    public async Task<IEnumerable<IGrouping<int, Player>>>
        GroupPlayersByScoreAsync(
            CancellationToken cancellationToken)
    {
        var players =
            await GetAllPlayersAsync(
                cancellationToken);

        return players
            .GroupBy(player => player.Score)
            .OrderByDescending(group => group.Key);
    }

    private async Task InvalidatePlayerCacheAsync(
        int playerId,
        CancellationToken cancellationToken)
    {
        await _cache.RemoveAsync(
            AllPlayersCacheKey,
            cancellationToken);

        await _cache.RemoveAsync(
            GetPlayerCacheKey(playerId),
            cancellationToken);

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