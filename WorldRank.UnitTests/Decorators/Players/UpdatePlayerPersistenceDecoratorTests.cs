using Moq;
using WorldRank.Application.Caching;
using WorldRank.Application.Infrastructure.Players;
using WorldRank.Domain.Player;
using WorldRank.Infrastructure.Decorators.Players;
using Xunit;

namespace WorldRank.UnitTests.Decorators.Players;

public sealed class UpdatePlayerPersistenceDecoratorTests
{
    [Fact]
    public async Task UpdateAsync_UpdateSucceeds_UpdatesRelevantCacheEntries()
    {
        // Arrange
        var inner =
            new Mock<IUpdatePlayerPersistence>();

        var cache =
            new Mock<ICache>();

        var cancellationToken =
            new CancellationTokenSource().Token;

        var player =
            new Player(10, "Michail");

        player.AddScore(100);

        inner
            .Setup(x => x.UpdateAsync(
                player,
                cancellationToken))
            .ReturnsAsync(true);

        cache
            .Setup(x => x.SetAsync(
                CacheKeys.PlayerById(player.Id),
                player,
                It.IsAny<TimeSpan>(),
                cancellationToken))
            .Returns(Task.CompletedTask);

        cache
            .Setup(x => x.RemoveAsync(
                CacheKeys.AllPlayers,
                cancellationToken))
            .Returns(Task.CompletedTask);

        var decorator =
            new UpdatePlayerPersistenceDecorator(
                inner.Object,
                cache.Object);

        // Act
        var result = await decorator.UpdateAsync(
            player,
            cancellationToken);

        // Assert
        Assert.True(result);

        inner.Verify(
            x => x.UpdateAsync(
                player,
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

        cache.Verify(
            x => x.RemoveAsync(
                CacheKeys.AllPlayers,
                cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_UpdateFails_DoesNotChangeCache()
    {
        // Arrange
        var inner =
            new Mock<IUpdatePlayerPersistence>();

        var cache =
            new Mock<ICache>();

        var player =
            new Player(10, "Michail");

        inner
            .Setup(x => x.UpdateAsync(
                player,
                CancellationToken.None))
            .ReturnsAsync(false);

        var decorator =
            new UpdatePlayerPersistenceDecorator(
                inner.Object,
                cache.Object);

        // Act
        var result = await decorator.UpdateAsync(
            player,
            CancellationToken.None);

        // Assert
        Assert.False(result);

        cache.Verify(
            x => x.SetAsync(
                It.IsAny<string>(),
                It.IsAny<Player>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        cache.Verify(
            x => x.RemoveAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
