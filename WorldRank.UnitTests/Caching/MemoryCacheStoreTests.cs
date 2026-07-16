using Microsoft.Extensions.Caching.Memory;
using WorldRank.Infrastructure.Caching;
using Xunit;

namespace WorldRank.UnitTests.Caching;

public sealed class MemoryCacheStoreTests
{
    [Fact]
    public async Task SetAsync_ThenGetAsync_ReturnsCachedValue()
    {
        // Arrange
        using var memoryCache =
            new MemoryCache(
                new MemoryCacheOptions());

        var cacheStore =
            new MemoryCacheStore(memoryCache);

        const string key = "test:key";
        const string expectedValue = "cached-value";

        // Act
        await cacheStore.SetAsync(
            key,
            expectedValue,
            TimeSpan.FromMinutes(5),
            CancellationToken.None);

        var result =
            await cacheStore.GetAsync<string>(
                key,
                CancellationToken.None);

        // Assert
        Assert.Equal(
            expectedValue,
            result);
    }

    [Fact]
    public async Task GetAsync_KeyDoesNotExist_ReturnsNull()
    {
        // Arrange
        using var memoryCache =
            new MemoryCache(
                new MemoryCacheOptions());

        var cacheStore =
            new MemoryCacheStore(memoryCache);

        // Act
        var result =
            await cacheStore.GetAsync<string>(
                "missing:key",
                CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task RemoveAsync_KeyExists_RemovesCachedValue()
    {
        // Arrange
        using var memoryCache =
            new MemoryCache(
                new MemoryCacheOptions());

        var cacheStore =
            new MemoryCacheStore(memoryCache);

        const string key = "test:key";

        await cacheStore.SetAsync(
            key,
            "cached-value",
            TimeSpan.FromMinutes(5),
            CancellationToken.None);

        // Act
        await cacheStore.RemoveAsync(
            key,
            CancellationToken.None);

        var result =
            await cacheStore.GetAsync<string>(
                key,
                CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAsync_CancelledToken_ThrowsOperationCanceledException()
    {
        // Arrange
        using var memoryCache =
            new MemoryCache(
                new MemoryCacheOptions());

        var cacheStore =
            new MemoryCacheStore(memoryCache);

        using var cancellationTokenSource =
            new CancellationTokenSource();

        cancellationTokenSource.Cancel();

        // Act
        var action =
            () => cacheStore.GetAsync<string>(
                "test:key",
                cancellationTokenSource.Token);

        // Assert
        await Assert.ThrowsAnyAsync<
            OperationCanceledException>(action);
    }

    [Fact]
    public async Task SetAsync_CancelledToken_DoesNotStoreValue()
    {
        // Arrange
        using var memoryCache =
            new MemoryCache(
                new MemoryCacheOptions());

        var cacheStore =
            new MemoryCacheStore(memoryCache);

        using var cancellationTokenSource =
            new CancellationTokenSource();

        cancellationTokenSource.Cancel();

        // Act
        var action =
            () => cacheStore.SetAsync(
                "test:key",
                "cached-value",
                TimeSpan.FromMinutes(5),
                cancellationTokenSource.Token);

        // Assert
        await Assert.ThrowsAnyAsync<
            OperationCanceledException>(action);

        var result =
            await cacheStore.GetAsync<string>(
                "test:key",
                CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task RemoveAsync_CancelledToken_DoesNotRemoveValue()
    {
        // Arrange
        using var memoryCache =
            new MemoryCache(
                new MemoryCacheOptions());

        var cacheStore =
            new MemoryCacheStore(memoryCache);

        const string key = "test:key";

        await cacheStore.SetAsync(
            key,
            "cached-value",
            TimeSpan.FromMinutes(5),
            CancellationToken.None);

        using var cancellationTokenSource =
            new CancellationTokenSource();

        cancellationTokenSource.Cancel();

        // Act
        var action =
            () => cacheStore.RemoveAsync(
                key,
                cancellationTokenSource.Token);

        // Assert
        await Assert.ThrowsAnyAsync<
            OperationCanceledException>(action);

        var result =
            await cacheStore.GetAsync<string>(
                key,
                CancellationToken.None);

        Assert.Equal(
            "cached-value",
            result);
    }
}
