using Microsoft.EntityFrameworkCore;
using WorldRank.Application.Interfaces;
using WorldRank.Domain.Player;
using WorldRank.Infrastructure.Persistence.Context;

namespace WorldRank.Infrastructure.Repositories;

public class DBPlayerRepository : IPlayerRepository
{


    private readonly WorldRankDbContext _dbContext;

    public DBPlayerRepository(WorldRankDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void AddPlayer(Player player)
    {
        _dbContext.Players.Add(player);
        _dbContext.SaveChanges();

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

            return;
        }

        _dbContext.Players.Remove(player);
        _dbContext.SaveChanges();


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
