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

    public void Add(Wallet wallet)
    {
        try
        {
            _dbContext.Wallets.Add(wallet);
            _dbContext.SaveChanges();
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is SqlException sqlException &&
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

    public Wallet GetWallet(
        int playerId,
        Currency currency)
    {
        var wallet = _dbContext.Wallets
            .FirstOrDefault(item =>
                item.PlayerId == playerId &&
                item.Currency == currency);

        if (wallet is null)
        {
            throw new WalletNotFoundException(
                playerId,
                currency);
        }

        return wallet;
    }

    public Wallet[] GetAll()
    {
        return _dbContext.Wallets
            .AsNoTracking()
            .ToArray();
    }

    public List<Wallet> GetAllWalletsByPlayerId(
        int playerId)
    {
        return _dbContext.Wallets
            .AsNoTracking()
            .Where(item => item.PlayerId == playerId)
            .ToList();
    }

    public void UpdateBalance(
        int playerId,
        Currency currency,
        decimal newBalance)
    {
        var wallet = GetTrackedWallet(
            playerId,
            currency);

        wallet.SetBalance(newBalance);

        _dbContext.SaveChanges();


    }

    public void Deposit(
        int playerId,
        Currency currency,
        decimal amount)
    {
        var wallet = GetTrackedWallet(
            playerId,
            currency);

        wallet.Deposit(amount);

        _dbContext.SaveChanges();


    }

    public void Withdraw(
        int playerId,
        Currency currency,
        decimal amount)
    {
        var wallet = GetTrackedWallet(
            playerId,
            currency);

        wallet.Withdraw(amount);

        _dbContext.SaveChanges();


    }

    public void Block(
        int playerId,
        Currency currency)
    {
        var wallet = GetTrackedWallet(
            playerId,
            currency);

        wallet.Block();

        _dbContext.SaveChanges();


    }

    public void Unblock(
        int playerId,
        Currency currency)
    {
        var wallet = GetTrackedWallet(
            playerId,
            currency);

        wallet.Unblock();

        _dbContext.SaveChanges();


    }

    public void Update(Wallet wallet)
    {
        _dbContext.Wallets.Update(wallet);
        _dbContext.SaveChanges();
    }

    private Wallet GetTrackedWallet(
        int playerId,
        Currency currency)
    {
        var wallet = _dbContext.Wallets
            .FirstOrDefault(item =>
                item.PlayerId == playerId &&
                item.Currency == currency);

        if (wallet is null)
        {
            throw new WalletNotFoundException(
                playerId,
                currency);
        }

        return wallet;
    }
}