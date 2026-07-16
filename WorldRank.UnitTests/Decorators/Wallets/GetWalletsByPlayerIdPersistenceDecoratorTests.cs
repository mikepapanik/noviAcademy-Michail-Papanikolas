using Moq;
using WorldRank.Application.Caching;
using WorldRank.Application.Infrastructure.Wallets;
using WorldRank.Domain.Enums;
using WorldRank.Domain.Wallets;
using WorldRank.Infrastructure.Decorators.Wallets;
using Xunit;

namespace WorldRank.UnitTests.Decorators.Wallets;

public sealed class GetWalletsByPlayerIdPersistenceDecoratorTests
{
    [Fact]
    public async Task GetByPlayerIdAsync_CacheContainsWallets_ReturnsCachedWallets()
    {
        // Arrange
        var inner =
            new Mock<IGetWalletsByPlayerIdPersistence>();

        var cache =
            new Mock<ICache>();

        const int playerId = 1;

        IReadOnlyList<Wallet> cachedWallets =
            new[]
            {
                new Wallet(
                    10,
                    playerId,
                    Currency.EUR,
                    100m),

                new Wallet(
                    11,
                    playerId,
                    Currency.USD,
                    200m)
            };

        cache
            .Setup(x => x.GetAsync<IReadOnlyList<Wallet>>(
                CacheKeys.WalletsByPlayerId(playerId),
                CancellationToken.None))
            .ReturnsAsync(cachedWallets);

        var decorator =
            new GetWalletsByPlayerIdPersistenceDecorator(
                inner.Object,
                cache.Object);

        // Act
        var result =
            await decorator.GetByPlayerIdAsync(
                playerId,
                CancellationToken.None);

        // Assert
        Assert.Same(cachedWallets, result);

        inner.Verify(
            x => x.GetByPlayerIdAsync(
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        cache.Verify(
            x => x.SetAsync(
                It.IsAny<string>(),
                It.IsAny<IReadOnlyList<Wallet>>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetByPlayerIdAsync_CacheIsEmpty_GetsAndCachesWallets()
    {
        // Arrange
        var inner =
            new Mock<IGetWalletsByPlayerIdPersistence>();

        var cache =
            new Mock<ICache>();

        const int playerId = 1;

        var cancellationToken =
            new CancellationTokenSource().Token;

        IReadOnlyList<Wallet> wallets =
            new[]
            {
                new Wallet(
                    10,
                    playerId,
                    Currency.EUR,
                    100m),

                new Wallet(
                    11,
                    playerId,
                    Currency.USD,
                    200m)
            };

        cache
            .Setup(x => x.GetAsync<IReadOnlyList<Wallet>>(
                CacheKeys.WalletsByPlayerId(playerId),
                cancellationToken))
            .ReturnsAsync(
                (IReadOnlyList<Wallet>?)null);

        inner
            .Setup(x => x.GetByPlayerIdAsync(
                playerId,
                cancellationToken))
            .ReturnsAsync(wallets);

        cache
            .Setup(x => x.SetAsync(
                CacheKeys.WalletsByPlayerId(playerId),
                wallets,
                It.IsAny<TimeSpan>(),
                cancellationToken))
            .Returns(Task.CompletedTask);

        var decorator =
            new GetWalletsByPlayerIdPersistenceDecorator(
                inner.Object,
                cache.Object);

        // Act
        var result =
            await decorator.GetByPlayerIdAsync(
                playerId,
                cancellationToken);

        // Assert
        Assert.Same(wallets, result);

        inner.Verify(
            x => x.GetByPlayerIdAsync(
                playerId,
                cancellationToken),
            Times.Once);

        cache.Verify(
            x => x.SetAsync(
                CacheKeys.WalletsByPlayerId(playerId),
                wallets,
                It.Is<TimeSpan>(
                    expiration =>
                        expiration ==
                        TimeSpan.FromMinutes(5)),
                cancellationToken),
            Times.Once);
    }
}
