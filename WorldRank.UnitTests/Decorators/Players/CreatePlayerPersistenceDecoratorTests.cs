using Moq;
using WorldRank.Application.Caching;
using WorldRank.Application.Infrastructure.Players;
using WorldRank.Domain.Player;
using WorldRank.Infrastructure.Decorators.Players;
using Xunit;

namespace WorldRank.UnitTests.Decorators.Players;

public sealed class CreatePlayerPersistenceDecoratorTests
{
    [Fact]
    public async Task CreateAsync_PlayerCreated_UpdatesRelevantCacheEntries()
    {
        // Arrange
        var inner =
            new Mock<ICreatePlayerPersistence>();

        var cache =
            new Mock<ICache>();

        var cancellationToken =
            new CancellationTokenSource().Token;

        var player =
            new Player(10, "Michail");

        inner
            .Setup(x => x.CreateAsync(
                player,
                cancellationToken))
            .Returns(Task.CompletedTask);

        cache
            .Setup(x => x.RemoveAsync(
                CacheKeys.AllPlayers,
                cancellationToken))
            .Returns(Task.CompletedTask);

        cache
            .Setup(x => x.SetAsync(
                CacheKeys.PlayerById(player.Id),
                player,
                It.IsAny<TimeSpan>(),
                cancellationToken))
            .Returns(Task.CompletedTask);

        var decorator =
            new CreatePlayerPersistenceDecorator(
                inner.Object,
                cache.Object);

        // Act
        await decorator.CreateAsync(
            player,
            cancellationToken);

        // Assert
        inner.Verify(
            x => x.CreateAsync(
                player,
                cancellationToken),
            Times.Once);

        cache.Verify(
            x => x.RemoveAsync(
                CacheKeys.AllPlayers,
                cancellationToken),
            Times.Once);

        cache.Verify(
            x => x.SetAsync(
                CacheKeys.PlayerById(player.Id),
                player,
                It.Is<TimeSpan>(
                    expiration =>
                        expiration ==
                        TimeSpan.FromMinutes(5)),
                cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_InnerPersistenceFails_DoesNotChangeCache()
    {
        // Arrange
        var inner =
            new Mock<ICreatePlayerPersistence>();

        var cache =
            new Mock<ICache>();

        var player =
            new Player(10, "Michail");

        var expectedException =
            new InvalidOperationException(
                "Player creation failed.");

        inner
            .Setup(x => x.CreateAsync(
                player,
                CancellationToken.None))
            .ThrowsAsync(expectedException);

        var decorator =
            new CreatePlayerPersistenceDecorator(
                inner.Object,
                cache.Object);

        // Act
        var exception =
            await Assert.ThrowsAsync<
                InvalidOperationException>(
                () => decorator.CreateAsync(
                    player,
                    CancellationToken.None));

        // Assert
        Assert.Same(
            expectedException,
            exception);

        cache.Verify(
            x => x.RemoveAsync(
                It.IsAny<string>(),
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
}
