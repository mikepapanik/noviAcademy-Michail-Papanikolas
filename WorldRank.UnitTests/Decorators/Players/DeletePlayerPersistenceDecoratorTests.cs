using Moq;
using WorldRank.Application.Caching;
using WorldRank.Application.Infrastructure.Players;
using WorldRank.Infrastructure.Decorators.Players;
using Xunit;

namespace WorldRank.UnitTests.Decorators.Players;

public sealed class DeletePlayerPersistenceDecoratorTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task DeleteAsync_AlwaysRemovesPossiblyStaleCacheEntries(
        bool deleted)
    {
        // Arrange
        var inner =
            new Mock<IDeletePlayerPersistence>();

        var cache =
            new Mock<ICache>();

        const int playerId = 10;

        var cancellationToken =
            new CancellationTokenSource().Token;

        inner
            .Setup(x => x.DeleteAsync(
                playerId,
                cancellationToken))
            .ReturnsAsync(deleted);

        cache
            .Setup(x => x.RemoveAsync(
                CacheKeys.PlayerById(playerId),
                cancellationToken))
            .Returns(Task.CompletedTask);

        cache
            .Setup(x => x.RemoveAsync(
                CacheKeys.AllPlayers,
                cancellationToken))
            .Returns(Task.CompletedTask);

        var decorator =
            new DeletePlayerPersistenceDecorator(
                inner.Object,
                cache.Object);

        // Act
        var result = await decorator.DeleteAsync(
            playerId,
            cancellationToken);

        // Assert
        Assert.Equal(deleted, result);

        inner.Verify(
            x => x.DeleteAsync(
                playerId,
                cancellationToken),
            Times.Once);

        cache.Verify(
            x => x.RemoveAsync(
                CacheKeys.PlayerById(playerId),
                cancellationToken),
            Times.Once);

        cache.Verify(
            x => x.RemoveAsync(
                CacheKeys.AllPlayers,
                cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_InnerPersistenceFails_DoesNotChangeCache()
    {
        // Arrange
        var inner =
            new Mock<IDeletePlayerPersistence>();

        var cache =
            new Mock<ICache>();

        const int playerId = 10;

        var expectedException =
            new InvalidOperationException(
                "Player deletion failed.");

        inner
            .Setup(x => x.DeleteAsync(
                playerId,
                CancellationToken.None))
            .ThrowsAsync(expectedException);

        var decorator =
            new DeletePlayerPersistenceDecorator(
                inner.Object,
                cache.Object);

        // Act
        var exception =
            await Assert.ThrowsAsync<
                InvalidOperationException>(
                () => decorator.DeleteAsync(
                    playerId,
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
    }
}
