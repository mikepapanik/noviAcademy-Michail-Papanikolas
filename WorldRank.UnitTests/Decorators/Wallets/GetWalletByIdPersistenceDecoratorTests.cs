using Moq;
using WorldRank.Application.Caching;
using WorldRank.Application.Infrastructure.Wallets;
using WorldRank.Domain.Enums;
using WorldRank.Domain.Wallets;
using WorldRank.Infrastructure.Decorators.Wallets;
using Xunit;

namespace WorldRank.UnitTests.Decorators.Wallets;

public sealed class GetWalletByIdPersistenceDecoratorTests
{
    [Fact]
    public async Task GetByIdAsync_CacheContainsWallet_ReturnsCachedWallet()
    {
        // Arrange
        var inner =
            new Mock<IGetWalletByIdPersistence>();

        var cache =
            new Mock<ICache>();

        const int walletId = 10;

        var cachedWallet = new Wallet(
            walletId,
            1,
            Currency.EUR,
            100m);

        cache
            .Setup(x => x.GetAsync<Wallet>(
                CacheKeys.WalletById(walletId),
                CancellationToken.None))
            .ReturnsAsync(cachedWallet);

        var decorator =
            new GetWalletByIdPersistenceDecorator(
                inner.Object,
                cache.Object);

        // Act
        var result = await decorator.GetByIdAsync(
            walletId,
            CancellationToken.None);

        // Assert
        Assert.Same(cachedWallet, result);

        inner.Verify(
            x => x.GetByIdAsync(
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        cache.Verify(
            x => x.SetAsync(
                It.IsAny<string>(),
                It.IsAny<Wallet>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_CacheIsEmpty_GetsAndCachesWallet()
    {
        // Arrange
        var inner =
            new Mock<IGetWalletByIdPersistence>();

        var cache =
            new Mock<ICache>();

        const int walletId = 10;

        var cancellationToken =
            new CancellationTokenSource().Token;

        var wallet = new Wallet(
            walletId,
            1,
            Currency.EUR,
            100m);

        cache
            .Setup(x => x.GetAsync<Wallet>(
                CacheKeys.WalletById(walletId),
                cancellationToken))
            .ReturnsAsync((Wallet?)null);

        inner
            .Setup(x => x.GetByIdAsync(
                walletId,
                cancellationToken))
            .ReturnsAsync(wallet);

        cache
            .Setup(x => x.SetAsync(
                CacheKeys.WalletById(walletId),
                wallet,
                It.IsAny<TimeSpan>(),
                cancellationToken))
            .Returns(Task.CompletedTask);

        var decorator =
            new GetWalletByIdPersistenceDecorator(
                inner.Object,
                cache.Object);

        // Act
        var result = await decorator.GetByIdAsync(
            walletId,
            cancellationToken);

        // Assert
        Assert.Same(wallet, result);

        inner.Verify(
            x => x.GetByIdAsync(
                walletId,
                cancellationToken),
            Times.Once);

        cache.Verify(
            x => x.SetAsync(
                CacheKeys.WalletById(walletId),
                wallet,
                It.Is<TimeSpan>(
                    expiration =>
                        expiration ==
                        TimeSpan.FromMinutes(5)),
                cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WalletDoesNotExist_ReturnsNullWithoutCaching()
    {
        // Arrange
        var inner =
            new Mock<IGetWalletByIdPersistence>();

        var cache =
            new Mock<ICache>();

        const int walletId = 999;

        cache
            .Setup(x => x.GetAsync<Wallet>(
                CacheKeys.WalletById(walletId),
                CancellationToken.None))
            .ReturnsAsync((Wallet?)null);

        inner
            .Setup(x => x.GetByIdAsync(
                walletId,
                CancellationToken.None))
            .ReturnsAsync((Wallet?)null);

        var decorator =
            new GetWalletByIdPersistenceDecorator(
                inner.Object,
                cache.Object);

        // Act
        var result = await decorator.GetByIdAsync(
            walletId,
            CancellationToken.None);

        // Assert
        Assert.Null(result);

        inner.Verify(
            x => x.GetByIdAsync(
                walletId,
                CancellationToken.None),
            Times.Once);

        cache.Verify(
            x => x.SetAsync(
                It.IsAny<string>(),
                It.IsAny<Wallet>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
