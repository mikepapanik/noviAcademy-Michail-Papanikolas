using Microsoft.EntityFrameworkCore;
using NLog;
using WorldRank.Application.Interfaces;
using WorldRank.Domain.Enums;
using WorldRank.Domain.Exceptions;
using WorldRank.Domain.Wallets;
using WorldRank.Infrastructure.Persistence.Context;

namespace WorldRank.Infrastructure.Repositories;

public class DBWalletRepository : IWalletRepository
{
    private static readonly Logger _logger =
        LogManager.GetCurrentClassLogger();

    private readonly WorldRankDbContext _dbContext;

    public DBWalletRepository(WorldRankDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(Wallet wallet)
    {
        var exists = _dbContext.Wallets.Any(item =>
            item.PlayerId == wallet.PlayerId &&
            item.Currency == wallet.Currency);

        if (exists)
        {
            throw new DuplicateWalletException(
                wallet.PlayerId,
                wallet.Currency);
        }

        _dbContext.Wallets.Add(wallet);
        _dbContext.SaveChanges();

        _logger.Info(
            "Wallet {WalletId} created in database for player {PlayerId} in {Currency} with balance {Balance}",
            wallet.Id,
            wallet.PlayerId,
            wallet.Currency,
            wallet.Balance);
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

        _logger.Info(
            "Wallet balance updated in database for player {PlayerId} in {Currency}. New balance {Balance}",
            playerId,
            currency,
            newBalance);
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

        _logger.Info(
            "Deposit completed in database for player {PlayerId} in {Currency}. Amount {Amount}",
            playerId,
            currency,
            amount);
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

        _logger.Info(
            "Withdraw completed in database for player {PlayerId} in {Currency}. Amount {Amount}",
            playerId,
            currency,
            amount);
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

        _logger.Info(
            "Wallet blocked in database for player {PlayerId} in {Currency}",
            playerId,
            currency);
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

        _logger.Info(
            "Wallet unblocked in database for player {PlayerId} in {Currency}",
            playerId,
            currency);
    }

    public void SaveChanges()
    {
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