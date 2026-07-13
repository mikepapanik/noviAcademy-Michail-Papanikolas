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

    public async Task AddPlayerAsync(Player player,CancellationToken cancellationToken)
    {
        await _dbContext.Players.AddAsync(player,cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Player>> GetAllPlayersAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Players
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Player?> FindPlayerAsync(int playerId,CancellationToken cancellationToken)
    {
        return await _dbContext.Players
            .AsNoTracking()
            .FirstOrDefaultAsync(
                player => player.Id == playerId,cancellationToken);
    }

    public async Task DeletePlayerAsync(int playerId,CancellationToken cancellationToken)
    {
        var player = await _dbContext.Players
            .FirstOrDefaultAsync(
                item => item.Id == playerId,cancellationToken);

        if (player is null)
        {
            return;
        }

        _dbContext.Players.Remove(player);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
