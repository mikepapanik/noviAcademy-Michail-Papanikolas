using Moq;
using WorldRank.Application.Caching;
using WorldRank.Application.Infrastructure.Wallets;
using WorldRank.Domain.Enums;
using WorldRank.Domain.Wallets;
using WorldRank.Infrastructure.Decorators.Wallets;
using Xunit;

namespace WorldRank.UnitTests.Decorators.Wallets;

public sealed class UpdateWalletPersistenceDecoratorTests
{
    [Fact]
    public async Task UpdateAsync_UpdateSucceeds_UpdatesRelevantCacheEntries()
    {
        // Arrange
        var inner =
            new Mock<IUpdateWalletPersistence>();

        var cache =
            new Mock<ICache>();

        var cancellationToken =
            new CancellationTokenSource().Token;

        var wallet = new Wallet(
            10,
            1,
            Currency.EUR,
            100m);

        wallet.Deposit(50m);

        inner
            .Setup(x => x.UpdateAsync(
                wallet,
                cancellationToken))
            .Returns(Task.CompletedTask);

        cache
            .Setup(x => x.SetAsync(
                CacheKeys.WalletById(wallet.Id),
                wallet,
                It.IsAny<TimeSpan>(),
                cancellationToken))
            .Returns(Task.CompletedTask);

        cache
            .Setup(x => x.RemoveAsync(
                CacheKeys.WalletsByPlayerId(
                    wallet.PlayerId),
                cancellationToken))
            .Returns(Task.CompletedTask);

        var decorator =
            new UpdateWalletPersistenceDecorator(
                inner.Object,
                cache.Object);

        // Act
        await decorator.UpdateAsync(
            wallet,
            cancellationToken);

        // Assert
        inner.Verify(
            x => x.UpdateAsync(
                wallet,
                cancellationToken),
            Times.Once);

        cache.Verify(
            x => x.SetAsync(
                CacheKeys.WalletById(wallet.Id),
                wallet,
                It.Is<TimeSpan>(
                    expiration =>
                        expiration ==
                        TimeSpan.FromMinutes(5)),
                cancellationToken),
            Times.Once);

        cache.Verify(
            x => x.RemoveAsync(
                CacheKeys.WalletsByPlayerId(
                    wallet.PlayerId),
                cancellationToken),
            Times.Once);

        Assert.Equal(150m, wallet.Balance);
    }

    [Fact]
    public async Task UpdateAsync_InnerPersistenceFails_DoesNotChangeCache()
    {
        // Arrange
        var inner =
            new Mock<IUpdateWalletPersistence>();

        var cache =
            new Mock<ICache>();

        var wallet = new Wallet(
            10,
            1,
            Currency.EUR,
            100m);

        var expectedException =
            new InvalidOperationException(
                "Wallet update failed.");

        inner
            .Setup(x => x.UpdateAsync(
                wallet,
                CancellationToken.None))
            .ThrowsAsync(expectedException);

        var decorator =
            new UpdateWalletPersistenceDecorator(
                inner.Object,
                cache.Object);

        // Act
        var exception =
            await Assert.ThrowsAsync<
                InvalidOperationException>(
                () => decorator.UpdateAsync(
                    wallet,
                    CancellationToken.None));

        // Assert
        Assert.Same(
            expectedException,
            exception);

        cache.Verify(
            x => x.SetAsync(
                It.IsAny<string>(),
                It.IsAny<Wallet>(),
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
