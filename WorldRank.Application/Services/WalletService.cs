using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using WorldRank.Application.Interfaces;
using WorldRank.Application.Strategies;
using WorldRank.Domain.Enums;
using WorldRank.Domain.Exceptions;
using WorldRank.Domain.Wallets;

namespace WorldRank.Application.Services;

public class WalletService
{
    private static readonly TimeSpan CacheDuration =
        TimeSpan.FromMinutes(5);

    private readonly IWalletRepository _walletRepository;
    private readonly IPlayerRepository _playerRepository;

    private readonly IReadOnlyDictionary<
        FundsOperation,
        IFundsStrategy> _fundsStrategies;

    private readonly IMemoryCache _cache;
    private readonly ILogger<WalletService> _logger;

    public WalletService(
        IWalletRepository walletRepository,
        IPlayerRepository playerRepository,
        IEnumerable<IFundsStrategy> strategies,
        IMemoryCache cache,
        ILogger<WalletService> logger)
    {
        _walletRepository = walletRepository;
        _playerRepository = playerRepository;

        _fundsStrategies = strategies.ToDictionary(
            strategy => strategy.Operation);

        _cache = cache;
        _logger = logger;
    }

    public void AddWalletToPlayer(
        int id,
        int playerId,
        Currency currency,
        decimal balance)
    {
        if (_playerRepository.FindPlayer(playerId) is null)
        {
            throw new PlayerNotFoundException(playerId);
        }

        var wallet = new Wallet(
            id,
            playerId,
            currency,
            balance);

        _walletRepository.Add(wallet);

        InvalidateWalletCache(playerId);

        _logger.LogInformation(
            "Wallet {WalletId} created for player {PlayerId} in {Currency} with balance {Balance}",
            id,
            playerId,
            currency,
            balance);
    }

    public List<Wallet> GetAllWalletsByPlayerId(
        int playerId)
    {
        var cacheKey =
            GetWalletsCacheKey(playerId);

        if (_cache.TryGetValue(
            cacheKey,
            out List<Wallet>? cachedWallets))
        {
            _logger.LogInformation(
                "Wallets for player {PlayerId} loaded from cache",
                playerId);

            return cachedWallets!;
        }

        var wallets = _walletRepository
            .GetAllWalletsByPlayerId(playerId)
            .ToList();

        _cache.Set(
            cacheKey,
            wallets,
            CacheDuration);

        _logger.LogInformation(
            "Wallets for player {PlayerId} loaded from repository and stored in cache",
            playerId);

        return wallets;
    }

    public void Deposit(
        int playerId,
        Currency currency,
        decimal amount)
    {
        ExecuteFundsOperation(
            playerId,
            currency,
            amount,
            FundsOperation.Add);
    }

    public void Withdraw(
        int playerId,
        Currency currency,
        decimal amount)
    {
        ExecuteFundsOperation(
            playerId,
            currency,
            amount,
            FundsOperation.Subtract);
    }

    public void ForceSubtract(
        int playerId,
        Currency currency,
        decimal amount)
    {
        ExecuteFundsOperation(
            playerId,
            currency,
            amount,
            FundsOperation.ForceSubtract);
    }

    public void UpdateBalance(
        int playerId,
        Currency currency,
        decimal newBalance)
    {
        _walletRepository.UpdateBalance(
            playerId,
            currency,
            newBalance);

        InvalidateWalletCache(playerId);

        _logger.LogInformation(
            "Wallet balance updated for player {PlayerId} in {Currency}. New balance: {Balance}",
            playerId,
            currency,
            newBalance);
    }

    public void Block(
        int playerId,
        Currency currency)
    {
        _walletRepository.Block(
            playerId,
            currency);

        InvalidateWalletCache(playerId);

        _logger.LogInformation(
            "Wallet blocked for player {PlayerId} in {Currency}",
            playerId,
            currency);
    }

    public void Unblock(
        int playerId,
        Currency currency)
    {
        _walletRepository.Unblock(
            playerId,
            currency);

        InvalidateWalletCache(playerId);

        _logger.LogInformation(
            "Wallet unblocked for player {PlayerId} in {Currency}",
            playerId,
            currency);
    }

    private void ExecuteFundsOperation(
        int playerId,
        Currency currency,
        decimal amount,
        FundsOperation operation)
    {
        var wallet = _walletRepository.GetWallet(
            playerId,
            currency);

        if (!_fundsStrategies.TryGetValue(
            operation,
            out var strategy))
        {
            throw new InvalidOperationException(
                $"No funds strategy registered for operation {operation}.");
        }

        strategy.Execute(
            wallet,
            amount);

        _walletRepository.Update(wallet);

        InvalidateWalletCache(playerId);

        _logger.LogInformation(
            "Funds operation {Operation} executed for player {PlayerId} in {Currency} with amount {Amount}",
            operation,
            playerId,
            currency,
            amount);
    }

    private void InvalidateWalletCache(int playerId)
    {
        _cache.Remove(
            GetWalletsCacheKey(playerId));

        _logger.LogInformation(
            "Wallet cache invalidated for player {PlayerId}",
            playerId);
    }

    private static string GetWalletsCacheKey(
        int playerId)
    {
        return $"wallets:player:{playerId}";
    }
}