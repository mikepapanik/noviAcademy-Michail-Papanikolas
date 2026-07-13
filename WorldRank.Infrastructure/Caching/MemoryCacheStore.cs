using Microsoft.Extensions.Caching.Memory;
using WorldRank.Application.Caching;

namespace WorldRank.Infrastructure.Caching;

public class MemoryCacheStore : ICache
{
    private readonly IMemoryCache _memoryCache;

    public MemoryCacheStore(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public Task<T?> GetAsync<T>(
        string key,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _memoryCache.TryGetValue(
            key,
            out T? value);

        return Task.FromResult(value);
    }

    public Task SetAsync<T>(
        string key,
        T value,
        TimeSpan expiration,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _memoryCache.Set(
            key,
            value,
            expiration);

        return Task.CompletedTask;
    }

    public Task RemoveAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _memoryCache.Remove(key);

        return Task.CompletedTask;
    }
}
