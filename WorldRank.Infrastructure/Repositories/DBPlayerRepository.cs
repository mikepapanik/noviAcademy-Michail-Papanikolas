using Microsoft.EntityFrameworkCore;
using NLog;
using WorldRank.Application.Interfaces;
using WorldRank.Domain.Player;
using WorldRank.Infrastructure.Persistence.Context;

namespace WorldRank.Infrastructure.Repositories;

public class DBPlayerRepository : IPlayerRepository
{
    private static readonly Logger _logger =
        LogManager.GetCurrentClassLogger();

    private readonly WorldRankDbContext _dbContext;

    public DBPlayerRepository(WorldRankDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void AddPlayer(Player player)
    {
        _dbContext.Players.Add(player);
        _dbContext.SaveChanges();

        _logger.Info(
            "Player {PlayerId} ({Name}) added to database with score {Score}",
            player.Id,
            player.Name,
            player.Score);
    }

    public IEnumerable<Player> GetAllPlayers()
    {
        return _dbContext.Players
            .AsNoTracking()
            .ToList();
    }

    public void DeletePlayer(int playerId)
    {
        var player = _dbContext.Players
            .FirstOrDefault(item => item.Id == playerId);

        if (player is null)
        {
            _logger.Warn(
                "Delete skipped: player {PlayerId} not found in database",
                playerId);

            return;
        }

        _dbContext.Players.Remove(player);
        _dbContext.SaveChanges();

        _logger.Info(
            "Player {PlayerId} deleted from database",
            playerId);
    }

    public Player? FindPlayer(int playerId)
    {
        return _dbContext.Players
            .AsNoTracking()
            .FirstOrDefault(item => item.Id == playerId);
    }

    public IEnumerable<IGrouping<int, Player>> GroupPlayersByScore()
    {
        return _dbContext.Players
            .AsNoTracking()
            .ToList()
            .GroupBy(player => player.Score)
            .OrderByDescending(group => group.Key);
    }
}
