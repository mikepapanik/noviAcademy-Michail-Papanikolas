using Microsoft.Extensions.Logging;
using Moq;
using WorldRank.Application.Caching;
using WorldRank.Application.Interfaces;
using WorldRank.Application.Services;
using WorldRank.Application.Strategies;
using WorldRank.Domain.Enums;
using WorldRank.Domain.Exceptions;
using WorldRank.Domain.Player;
using WorldRank.Domain.Wallets;

namespace WorldRank.UnitTests.Services;

public sealed class WalletServiceTests
{
    private readonly Mock<IWalletRepository> _walletRepository;
    private readonly Mock<IPlayerRepository> _playerRepository;
    private readonly Mock<ICache> _cache;
    private readonly Mock<ILogger<WalletService>> _logger;

    public WalletServiceTests()
    {
        _walletRepository = new Mock<IWalletRepository>();
        _playerRepository = new Mock<IPlayerRepository>();
        _cache = new Mock<ICache>();
        _logger = new Mock<ILogger<WalletService>>();
    }

    [Fact]
    public async Task AddWalletToPlayerAsync_ExistingPlayer_AddsWalletAndUpdatesCache()
    {
        // Arrange
        var player = new Player(10, "Michail");

        _playerRepository
            .Setup(repository => repository.FindPlayerAsync(
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(player);

        var service = CreateService();

        // Act
        var result = await service.AddWalletToPlayerAsync(
            id: 1,
            playerId: 10,
            currency: Currency.EUR,
            balance: 100m,
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.Equal(1, result.Id);
        Assert.Equal(10, result.PlayerId);
        Assert.Equal(Currency.EUR, result.Currency);
        Assert.Equal(100m, result.Balance);

        _walletRepository.Verify(
            repository => repository.AddAsync(
                result,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _cache.Verify(
            cache => cache.SetAsync(
                "wallets:1",
                result,
                TimeSpan.FromSeconds(60),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _cache.Verify(
            cache => cache.RemoveAsync(
                "wallets:player:10",
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task AddWalletToPlayerAsync_PlayerDoesNotExist_ThrowsPlayerNotFoundException()
    {
        // Arrange
        _playerRepository
            .Setup(repository => repository.FindPlayerAsync(
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Player?)null);

        var service = CreateService();

        // Act
        var action = async () =>
            await service.AddWalletToPlayerAsync(
                id: 1,
                playerId: 10,
                currency: Currency.EUR,
                balance: 100m,
                cancellationToken: CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<PlayerNotFoundException>(action);

        _walletRepository.Verify(
            repository => repository.AddAsync(
                It.IsAny<Wallet>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _cache.Verify(
            cache => cache.SetAsync(
                It.IsAny<string>(),
                It.IsAny<Wallet>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetWalletByIdAsync_CacheHit_ReturnsCachedWallet()
    {
        // Arrange
        var wallet = CreateWallet();

        _cache
            .Setup(cache => cache.GetAsync<Wallet>(
                "wallets:1",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(wallet);

        var service = CreateService();

        // Act
        var result = await service.GetWalletByIdAsync(
            1,
            CancellationToken.None);

        // Assert
        Assert.Same(wallet, result);

        _walletRepository.Verify(
            repository => repository.GetByIdAsync(
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetWalletByIdAsync_CacheMiss_ReturnsRepositoryWalletAndCachesIt()
    {
        // Arrange
        var wallet = CreateWallet();

        _cache
            .Setup(cache => cache.GetAsync<Wallet>(
                "wallets:1",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Wallet?)null);

        _walletRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(wallet);

        var service = CreateService();

        // Act
        var result = await service.GetWalletByIdAsync(
            1,
            CancellationToken.None);

        // Assert
        Assert.Same(wallet, result);

        _cache.Verify(
            cache => cache.SetAsync(
                "wallets:1",
                wallet,
                TimeSpan.FromSeconds(60),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetWalletByIdAsync_WalletDoesNotExist_ReturnsNullWithoutCaching()
    {
        // Arrange
        _cache
            .Setup(cache => cache.GetAsync<Wallet>(
                "wallets:1",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Wallet?)null);

        _walletRepository
            .Setup(repository => repository.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Wallet?)null);

        var service = CreateService();

        // Act
        var result = await service.GetWalletByIdAsync(
            1,
            CancellationToken.None);

        // Assert
        Assert.Null(result);

        _cache.Verify(
            cache => cache.SetAsync(
                It.IsAny<string>(),
                It.IsAny<Wallet>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetAllWalletsByPlayerIdAsync_CacheHit_ReturnsCachedWallets()
    {
        // Arrange
        IReadOnlyList<Wallet> wallets =
        [
            CreateWallet(),
            new Wallet(
                id: 2,
                playerId: 10,
                currency: Currency.USD,
                balance: 200m)
        ];

        _cache
            .Setup(cache => cache.GetAsync<IReadOnlyList<Wallet>>(
                "wallets:player:10",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(wallets);

        var service = CreateService();

        // Act
        var result = await service.GetAllWalletsByPlayerIdAsync(
            10,
            CancellationToken.None);

        // Assert
        Assert.Same(wallets, result);

        _walletRepository.Verify(
            repository => repository.GetAllWalletsByPlayerIdAsync(
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetAllWalletsByPlayerIdAsync_CacheMiss_LoadsAndCachesWallets()
    {
        // Arrange
        IReadOnlyList<Wallet> wallets =
        [
            CreateWallet()
        ];

        _cache
            .Setup(cache => cache.GetAsync<IReadOnlyList<Wallet>>(
                "wallets:player:10",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyList<Wallet>?)null);

        _walletRepository
            .Setup(repository => repository.GetAllWalletsByPlayerIdAsync(
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(wallets);

        var service = CreateService();

        // Act
        var result = await service.GetAllWalletsByPlayerIdAsync(
            10,
            CancellationToken.None);

        // Assert
        Assert.Same(wallets, result);

        _cache.Verify(
            cache => cache.SetAsync(
                "wallets:player:10",
                wallets,
                TimeSpan.FromSeconds(60),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DepositAsync_RegisteredAddStrategy_ExecutesCorrectStrategy()
    {
        // Arrange
        var wallet = CreateWallet();
        var strategy = CreateStrategy(FundsOperation.Add);

        SetupExistingWallet(wallet);

        var service = CreateService(strategy.Object);

        // Act
        var result = await service.DepositAsync(
            wallet.Id,
            25m,
            CancellationToken.None);

        // Assert
        Assert.Same(wallet, result);

        strategy.Verify(
            item => item.Execute(wallet, 25m),
            Times.Once);

        VerifyWalletWasUpdated(wallet);
    }

    [Fact]
    public async Task WithdrawAsync_RegisteredSubtractStrategy_ExecutesCorrectStrategy()
    {
        // Arrange
        var wallet = CreateWallet();
        var strategy = CreateStrategy(FundsOperation.Subtract);

        SetupExistingWallet(wallet);

        var service = CreateService(strategy.Object);

        // Act
        var result = await service.WithdrawAsync(
            wallet.Id,
            25m,
            CancellationToken.None);

        // Assert
        Assert.Same(wallet, result);

        strategy.Verify(
            item => item.Execute(wallet, 25m),
            Times.Once);

        VerifyWalletWasUpdated(wallet);
    }

    [Fact]
    public async Task ForceSubtractAsync_RegisteredForceSubtractStrategy_ExecutesCorrectStrategy()
    {
        // Arrange
        var wallet = CreateWallet();
        var strategy = CreateStrategy(
            FundsOperation.ForceSubtract);

        SetupExistingWallet(wallet);

        var service = CreateService(strategy.Object);

        // Act
        var result = await service.ForceSubtractAsync(
            wallet.Id,
            150m,
            CancellationToken.None);

        // Assert
        Assert.Same(wallet, result);

        strategy.Verify(
            item => item.Execute(wallet, 150m),
            Times.Once);

        VerifyWalletWasUpdated(wallet);
    }

    [Fact]
    public async Task DepositAsync_WalletDoesNotExist_ThrowsWalletNotFoundException()
    {
        // Arrange
        _walletRepository
            .Setup(repository => repository.GetByIdAsync(
                999,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Wallet?)null);

        var service = CreateService(
            new AddFundsStrategy());

        // Act
        var action = async () =>
            await service.DepositAsync(
                999,
                20m,
                CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<WalletNotFoundException>(
            action);

        _walletRepository.Verify(
            repository => repository.UpdateAsync(
                It.IsAny<Wallet>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task DepositAsync_AddStrategyNotRegistered_ThrowsInvalidOperationException()
    {
        // Arrange
        var wallet = CreateWallet();

        SetupExistingWallet(wallet);

        var service = CreateService();

        // Act
        var action = async () =>
            await service.DepositAsync(
                wallet.Id,
                20m,
                CancellationToken.None);

        // Assert
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                action);

        Assert.Contains(
            "No funds strategy registered",
            exception.Message);

        _walletRepository.Verify(
            repository => repository.UpdateAsync(
                It.IsAny<Wallet>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateBalanceAsync_ExistingWallet_UpdatesBalanceAndPersistence()
    {
        // Arrange
        var wallet = CreateWallet();

        SetupExistingWallet(wallet);

        var service = CreateService();

        // Act
        var result = await service.UpdateBalanceAsync(
            wallet.Id,
            250m,
            CancellationToken.None);

        // Assert
        Assert.Equal(250m, result.Balance);

        VerifyWalletWasUpdated(wallet);
    }

    [Fact]
    public async Task BlockAsync_ExistingWallet_BlocksAndUpdatesWallet()
    {
        // Arrange
        var wallet = CreateWallet();

        SetupExistingWallet(wallet);

        var service = CreateService();

        // Act
        var result = await service.BlockAsync(
            wallet.Id,
            CancellationToken.None);

        // Assert
        Assert.True(result.IsBlocked);

        VerifyWalletWasUpdated(wallet);
    }

    [Fact]
    public async Task UnblockAsync_BlockedWallet_UnblocksAndUpdatesWallet()
    {
        // Arrange
        var wallet = new Wallet(
            id: 1,
            playerId: 10,
            currency: Currency.EUR,
            balance: 100m,
            isBlocked: true);

        SetupExistingWallet(wallet);

        var service = CreateService();

        // Act
        var result = await service.UnblockAsync(
            wallet.Id,
            CancellationToken.None);

        // Assert
        Assert.False(result.IsBlocked);

        VerifyWalletWasUpdated(wallet);
    }

    private WalletService CreateService(
        params IFundsStrategy[] strategies)
    {
        return new WalletService(
            _walletRepository.Object,
            _playerRepository.Object,
            strategies,
            _cache.Object,
            _logger.Object);
    }

    private static Wallet CreateWallet()
    {
        return new Wallet(
            id: 1,
            playerId: 10,
            currency: Currency.EUR,
            balance: 100m);
    }

    private static Mock<IFundsStrategy> CreateStrategy(
        FundsOperation operation)
    {
        var strategy = new Mock<IFundsStrategy>();

        strategy
            .SetupGet(item => item.Operation)
            .Returns(operation);

        return strategy;
    }

    private void SetupExistingWallet(
        Wallet wallet)
    {
        _walletRepository
            .Setup(repository => repository.GetByIdAsync(
                wallet.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(wallet);
    }

    private void VerifyWalletWasUpdated(
        Wallet wallet)
    {
        _walletRepository.Verify(
            repository => repository.UpdateAsync(
                wallet,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _cache.Verify(
            cache => cache.SetAsync(
                $"wallets:{wallet.Id}",
                wallet,
                TimeSpan.FromSeconds(60),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _cache.Verify(
            cache => cache.RemoveAsync(
                $"wallets:player:{wallet.PlayerId}",
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
