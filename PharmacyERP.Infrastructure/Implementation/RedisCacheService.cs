using System.Collections.Concurrent;
using System.Text.Json;
using Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace Infrastructure.Services;

public sealed class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    private static readonly ConcurrentDictionary<string, byte> _keys = new();

    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetAsync<T>(
        string key,
        CancellationToken cancellationToken = default)
    {
        var json = await _cache.GetStringAsync(key, cancellationToken);
        if (string.IsNullOrWhiteSpace(json))
            return default;
        return JsonSerializer.Deserialize<T>(json);
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan expiration,
        CancellationToken cancellationToken = default)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration
        };

        var json = JsonSerializer.Serialize(value);

        await _cache.SetStringAsync(
            key,
            json,
            options,
            cancellationToken);

        _keys.TryAdd(key, 0);
    }

    public async Task RemoveAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync(key, cancellationToken);
        _keys.TryRemove(key, out _);
    }

    public async Task RemoveByPatternAsync(
        string pattern,
        CancellationToken cancellationToken = default)
    {
        var prefix = pattern.TrimEnd('*');

        var matchingKeys = _keys.Keys
            .Where(k => k.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            .ToList();

        foreach (var key in matchingKeys)
        {
            await _cache.RemoveAsync(key, cancellationToken);
            _keys.TryRemove(key, out _);
        }
    }
}