using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WorldRank.Application.Interfaces;
using WorldRank.Domain.Enums;
using WorldRank.Domain.Exceptions;
using WorldRank.Domain.Wallets;
using WorldRank.Infrastructure.Persistence.Context;

namespace WorldRank.Infrastructure.Repositories;

public class DBWalletRepository : IWalletRepository
{
    private readonly WorldRankDbContext _dbContext;

    public DBWalletRepository(WorldRankDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Wallet wallet,CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.Wallets.AddAsync(wallet,cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
            when (exception.InnerException is SqlException sqlException &&
                  (sqlException.Number == 2601 ||
                   sqlException.Number == 2627))
        {
            _dbContext.Entry(wallet).State =
                EntityState.Detached;

            throw new DuplicateWalletException(
                wallet.PlayerId,
                wallet.Currency);
        }
    }

    public async Task<Wallet?> GetByIdAsync(int walletId,CancellationToken cancellationToken)
    {
        return await _dbContext.Wallets
            .AsNoTracking()
            .FirstOrDefaultAsync(
                wallet => wallet.Id == walletId,
                cancellationToken);
    }

    public async Task<Wallet?> GetWalletAsync(int playerId,Currency currency,CancellationToken cancellationToken)
    {
        return await _dbContext.Wallets
            .AsNoTracking()
            .FirstOrDefaultAsync(
                wallet =>
                    wallet.PlayerId == playerId &&
                    wallet.Currency == currency,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Wallet>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Wallets
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Wallet>>
        GetAllWalletsByPlayerIdAsync(int playerId,CancellationToken cancellationToken)
    {
        return await _dbContext.Wallets
            .AsNoTracking()
            .Where(wallet =>
                wallet.PlayerId == playerId)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(Wallet wallet,CancellationToken cancellationToken)
    {
        _dbContext.Wallets.Update(wallet);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}