using Microsoft.Extensions.Caching.Hybrid;

namespace Shared.Redis;

public static class HybridCacheExtensions
{
    private static readonly HybridCacheEntryOptions forceNoWriteOptions = new()
    {
        Flags = HybridCacheEntryFlags.DisableLocalCacheWrite |
        HybridCacheEntryFlags.DisableDistributedCacheWrite
    };

    public static async ValueTask<(bool Exists, TValue? Value)> TryGetAsync<TValue>(this HybridCache cache, string key, CancellationToken ct = default)
    {
        var exists = true;
        var value = await cache.GetOrCreateAsync(
            key,
            _ =>
            {
                exists = false;
                return new ValueTask<TValue>(default(TValue)!);
            },
            forceNoWriteOptions,
            cancellationToken: ct
        );

        return (exists, value);
    }
    
    public static async ValueTask<bool> ExistsAsync<TValue>(this HybridCache cache, string key, CancellationToken ct = default)
    {
    	(var exists, _) = await cache.TryGetAsync<TValue>(key, ct);
    	return exists;
    }
}