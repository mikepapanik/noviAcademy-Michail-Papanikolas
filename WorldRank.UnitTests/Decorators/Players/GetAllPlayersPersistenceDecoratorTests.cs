using Moq;
using WorldRank.Application.Caching;
using WorldRank.Application.Infrastructure.Players;
using WorldRank.Domain.Player;
using WorldRank.Infrastructure.Decorators.Players;
using Xunit;

namespace WorldRank.UnitTests.Decorators.Players;

public sealed class GetAllPlayersPersistenceDecoratorTests
{
    [Fact]
    public async Task GetAllAsync_CacheContainsPlayers_ReturnsCachedPlayers()
    {
        // Arrange
        var inner =
            new Mock<IGetAllPlayersPersistence>();

        var cache =
            new Mock<ICache>();

        IReadOnlyList<Player> cachedPlayers =
            new[]
            {
                new Player(1, "Michail"),
                new Player(2, "Nikos")
            };

        cache
            .Setup(x => x.GetAsync<IReadOnlyList<Player>>(
                CacheKeys.AllPlayers,
                CancellationToken.None))
            .ReturnsAsync(cachedPlayers);

        var decorator =
            new GetAllPlayersPersistenceDecorator(
                inner.Object,
                cache.Object);

        // Act
        var result = await decorator.GetAllAsync(
            CancellationToken.None);

        // Assert
        Assert.Same(cachedPlayers, result);

        inner.Verify(
            x => x.GetAllAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);

        cache.Verify(
            x => x.SetAsync(
                It.IsAny<string>(),
                It.IsAny<IReadOnlyList<Player>>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetAllAsync_CacheIsEmpty_GetsAndCachesPlayers()
    {
        // Arrange
        var inner =
            new Mock<IGetAllPlayersPersistence>();

        var cache =
            new Mock<ICache>();

        var cancellationToken =
            new CancellationTokenSource().Token;

        IReadOnlyList<Player> players =
            new[]
            {
                new Player(1, "Michail"),
                new Player(2, "Nikos")
            };

        cache
            .Setup(x => x.GetAsync<IReadOnlyList<Player>>(
                CacheKeys.AllPlayers,
                cancellationToken))
            .ReturnsAsync(
                (IReadOnlyList<Player>?)null);

        inner
            .Setup(x => x.GetAllAsync(
                cancellationToken))
            .ReturnsAsync(players);

        cache
            .Setup(x => x.SetAsync(
                CacheKeys.AllPlayers,
                players,
                It.IsAny<TimeSpan>(),
                cancellationToken))
            .Returns(Task.CompletedTask);

        var decorator =
            new GetAllPlayersPersistenceDecorator(
                inner.Object,
                cache.Object);

        // Act
        var result = await decorator.GetAllAsync(
            cancellationToken);

        // Assert
        Assert.Same(players, result);

        inner.Verify(
            x => x.GetAllAsync(
                cancellationToken),
            Times.Once);

        cache.Verify(
            x => x.SetAsync(
                CacheKeys.AllPlayers,
                players,
                It.Is<TimeSpan>(
                    expiration =>
                        expiration ==
                        TimeSpan.FromMinutes(5)),
                cancellationToken),
            Times.Once);
    }
}
