using Moq;
using WorldRank.Application.Caching;
using WorldRank.Application.Infrastructure.Players;
using WorldRank.Domain.Player;
using WorldRank.Infrastructure.Decorators.Players;
using Xunit;

namespace WorldRank.UnitTests.Decorators.Players;

public sealed class GetPlayerByIdPersistenceDecoratorTests
{
    [Fact]
    public async Task GetByIdAsync_CacheContainsPlayer_ReturnsCachedPlayer()
    {
        // Arrange
        var inner =
            new Mock<IGetPlayerByIdPersistence>();

        var cache =
            new Mock<ICache>();

        const int playerId = 10;

        var cachedPlayer =
            new Player(playerId, "Michail");

        cache
            .Setup(x => x.GetAsync<Player>(
                CacheKeys.PlayerById(playerId),
                CancellationToken.None))
            .ReturnsAsync(cachedPlayer);

        var decorator =
            new GetPlayerByIdPersistenceDecorator(
                inner.Object,
                cache.Object);

        // Act
        var result = await decorator.GetByIdAsync(
            playerId,
            CancellationToken.None);

        // Assert
        Assert.Same(cachedPlayer, result);

        inner.Verify(
            x => x.GetByIdAsync(
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        cache.Verify(
            x => x.SetAsync(
                It.IsAny<string>(),
                It.IsAny<Player>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_CacheIsEmpty_GetsAndCachesPlayer()
    {
        // Arrange
        var inner =
            new Mock<IGetPlayerByIdPersistence>();

        var cache =
            new Mock<ICache>();

        const int playerId = 10;

        var cancellationToken =
            new CancellationTokenSource().Token;

        var player =
            new Player(playerId, "Michail");

        cache
            .Setup(x => x.GetAsync<Player>(
                CacheKeys.PlayerById(playerId),
                cancellationToken))
            .ReturnsAsync((Player?)null);

        inner
            .Setup(x => x.GetByIdAsync(
                playerId,
                cancellationToken))
            .ReturnsAsync(player);

        cache
            .Setup(x => x.SetAsync(
                CacheKeys.PlayerById(playerId),
                player,
                It.IsAny<TimeSpan>(),
                cancellationToken))
            .Returns(Task.CompletedTask);

        var decorator =
            new GetPlayerByIdPersistenceDecorator(
                inner.Object,
                cache.Object);

        // Act
        var result = await decorator.GetByIdAsync(
            playerId,
            cancellationToken);

        // Assert
        Assert.Same(player, result);

        inner.Verify(
            x => x.GetByIdAsync(
                playerId,
                cancellationToken),
            Times.Once);

        cache.Verify(
            x => x.SetAsync(
                CacheKeys.PlayerById(playerId),
                player,
                It.Is<TimeSpan>(
                    expiration =>
                        expiration ==
                        TimeSpan.FromMinutes(5)),
                cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_PlayerDoesNotExist_ReturnsNullWithoutCaching()
    {
        // Arrange
        var inner =
            new Mock<IGetPlayerByIdPersistence>();

        var cache =
            new Mock<ICache>();

        const int playerId = 999;

        cache
            .Setup(x => x.GetAsync<Player>(
                CacheKeys.PlayerById(playerId),
                CancellationToken.None))
            .ReturnsAsync((Player?)null);

        inner
            .Setup(x => x.GetByIdAsync(
                playerId,
                CancellationToken.None))
            .ReturnsAsync((Player?)null);

        var decorator =
            new GetPlayerByIdPersistenceDecorator(
                inner.Object,
                cache.Object);

        // Act
        var result = await decorator.GetByIdAsync(
            playerId,
            CancellationToken.None);

        // Assert
        Assert.Null(result);

        inner.Verify(
            x => x.GetByIdAsync(
                playerId,
                CancellationToken.None),
            Times.Once);

        cache.Verify(
            x => x.SetAsync(
                It.IsAny<string>(),
                It.IsAny<Player>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
