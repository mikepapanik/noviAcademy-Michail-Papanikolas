using Microsoft.Extensions.Logging;
using WorldRank.Application.Caching;
using WorldRank.Application.Interfaces;
using WorldRank.Application.Strategies;
using WorldRank.Domain.Enums;
using WorldRank.Domain.Exceptions;
using WorldRank.Domain.Wallets;

namespace WorldRank.Application.Services;

public class WalletService
{
    private static readonly TimeSpan CacheDuration =
        TimeSpan.FromSeconds(60);

    private readonly IWalletRepository _walletRepository;
    private readonly IPlayerRepository _playerRepository;

    private readonly IReadOnlyDictionary<
        FundsOperation,
        IFundsStrategy> _fundsStrategies;

    private readonly ICache _cache;
    private readonly ILogger<WalletService> _logger;

    public WalletService(
        IWalletRepository walletRepository,
        IPlayerRepository playerRepository,
        IEnumerable<IFundsStrategy> strategies,
        ICache cache,
        ILogger<WalletService> logger)
    {
        _walletRepository = walletRepository;
        _playerRepository = playerRepository;

        _fundsStrategies = strategies.ToDictionary(
            strategy => strategy.Operation);

        _cache = cache;
        _logger = logger;
    }

    public async Task<Wallet> AddWalletToPlayerAsync(
        int id,
        int playerId,
        Currency currency,
        decimal balance,
        CancellationToken cancellationToken)
    {
        if (await _playerRepository.FindPlayerAsync(
                playerId,
                cancellationToken) is null)
        {
            throw new PlayerNotFoundException(playerId);
        }

        var wallet = new Wallet(
            id,
            playerId,
            currency,
            balance);

        await _walletRepository.AddAsync(
            wallet,
            cancellationToken);

        await _cache.SetAsync(
            GetWalletCacheKey(wallet.Id),
            wallet,
            CacheDuration,
            cancellationToken);

        await InvalidateWalletListCacheAsync(
            playerId,
            cancellationToken);

        _logger.LogInformation(
            "Wallet {WalletId} created for player {PlayerId} in {Currency} with balance {Balance}",
            id,
            playerId,
            currency,
            balance);

        return wallet;
    }

    public async Task<Wallet?> GetWalletByIdAsync(
        int walletId,
        CancellationToken cancellationToken)
    {
        var cacheKey =
            GetWalletCacheKey(walletId);

        var cachedWallet =
            await _cache.GetAsync<Wallet>(
                cacheKey,
                cancellationToken);

        if (cachedWallet is not null)
        {
            _logger.LogInformation(
                "Wallet {WalletId} loaded from cache",
                walletId);

            return cachedWallet;
        }

        var wallet =
            await _walletRepository.GetByIdAsync(
                walletId,
                cancellationToken);

        if (wallet is not null)
        {
            await _cache.SetAsync(
                cacheKey,
                wallet,
                CacheDuration,
                cancellationToken);

            _logger.LogInformation(
                "Wallet {WalletId} loaded from repository and stored in cache",
                walletId);
        }

        return wallet;
    }

    public async Task<IReadOnlyList<Wallet>>
        GetAllWalletsByPlayerIdAsync(
            int playerId,
            CancellationToken cancellationToken)
    {
        var cacheKey =
            GetWalletsCacheKey(playerId);

        var cachedWallets =
            await _cache.GetAsync<IReadOnlyList<Wallet>>(
                cacheKey,
                cancellationToken);

        if (cachedWallets is not null)
        {
            _logger.LogInformation(
                "Wallets for player {PlayerId} loaded from cache",
                playerId);

            return cachedWallets;
        }

        var wallets = await _walletRepository
            .GetAllWalletsByPlayerIdAsync(
                playerId,
                cancellationToken);

        await _cache.SetAsync(
            cacheKey,
            wallets,
            CacheDuration,
            cancellationToken);

        _logger.LogInformation(
            "Wallets for player {PlayerId} loaded from repository and stored in cache",
            playerId);

        return wallets;
    }

    public async Task<Wallet> DepositAsync(
        int walletId,
        decimal amount,
        CancellationToken cancellationToken)
    {
        return await ExecuteFundsOperationAsync(
            walletId,
            amount,
            FundsOperation.Add,
            cancellationToken);
    }

    public async Task<Wallet> WithdrawAsync(
        int walletId,
        decimal amount,
        CancellationToken cancellationToken)
    {
        return await ExecuteFundsOperationAsync(
            walletId,
            amount,
            FundsOperation.Subtract,
            cancellationToken);
    }

    public async Task<Wallet> ForceSubtractAsync(
        int walletId,
        decimal amount,
        CancellationToken cancellationToken)
    {
        return await ExecuteFundsOperationAsync(
            walletId,
            amount,
            FundsOperation.ForceSubtract,
            cancellationToken);
    }

    public async Task<Wallet> UpdateBalanceAsync(
        int walletId,
        decimal newBalance,
        CancellationToken cancellationToken)
    {
        var wallet =
            await GetRequiredWalletAsync(
                walletId,
                cancellationToken);

        wallet.SetBalance(newBalance);

        await _walletRepository.UpdateAsync(
            wallet,
            cancellationToken);

        await UpdateWalletCacheAsync(
            wallet,
            cancellationToken);

        _logger.LogInformation(
            "Wallet {WalletId} balance updated. New balance: {Balance}",
            walletId,
            newBalance);

        return wallet;
    }

    public async Task<Wallet> BlockAsync(
        int walletId,
        CancellationToken cancellationToken)
    {
        var wallet =
            await GetRequiredWalletAsync(
                walletId,
                cancellationToken);

        wallet.Block();

        await _walletRepository.UpdateAsync(
            wallet,
            cancellationToken);

        await UpdateWalletCacheAsync(
            wallet,
            cancellationToken);

        _logger.LogInformation(
            "Wallet {WalletId} blocked",
            walletId);

        return wallet;
    }

    public async Task<Wallet> UnblockAsync(
        int walletId,
        CancellationToken cancellationToken)
    {
        var wallet =
            await GetRequiredWalletAsync(
                walletId,
                cancellationToken);

        wallet.Unblock();

        await _walletRepository.UpdateAsync(
            wallet,
            cancellationToken);

        await UpdateWalletCacheAsync(
            wallet,
            cancellationToken);

        _logger.LogInformation(
            "Wallet {WalletId} unblocked",
            walletId);

        return wallet;
    }

    private async Task<Wallet> ExecuteFundsOperationAsync(
        int walletId,
        decimal amount,
        FundsOperation operation,
        CancellationToken cancellationToken)
    {
        var wallet =
            await GetRequiredWalletAsync(
                walletId,
                cancellationToken);

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

        await _walletRepository.UpdateAsync(
            wallet,
            cancellationToken);

        await UpdateWalletCacheAsync(
            wallet,
            cancellationToken);

        _logger.LogInformation(
            "Funds operation {Operation} executed for wallet {WalletId} with amount {Amount}",
            operation,
            walletId,
            amount);

        return wallet;
    }

    private async Task<Wallet> GetRequiredWalletAsync(
        int walletId,
        CancellationToken cancellationToken)
    {
        var wallet =
            await _walletRepository.GetByIdAsync(
                walletId,
                cancellationToken);

        if (wallet is null)
        {
            throw new WalletNotFoundException(
                walletId);
        }

        return wallet;
    }

    private async Task UpdateWalletCacheAsync(
        Wallet wallet,
        CancellationToken cancellationToken)
    {
        await _cache.SetAsync(
            GetWalletCacheKey(wallet.Id),
            wallet,
            CacheDuration,
            cancellationToken);

        await InvalidateWalletListCacheAsync(
            wallet.PlayerId,
            cancellationToken);
    }

    private async Task InvalidateWalletListCacheAsync(
        int playerId,
        CancellationToken cancellationToken)
    {
        await _cache.RemoveAsync(
            GetWalletsCacheKey(playerId),
            cancellationToken);

        _logger.LogInformation(
            "Wallet list cache invalidated for player {PlayerId}",
            playerId);
    }

    private static string GetWalletCacheKey(
        int walletId)
    {
        return $"wallets:{walletId}";
    }

    private static string GetWalletsCacheKey(
        int playerId)
    {
        return $"wallets:player:{playerId}";
    }
}