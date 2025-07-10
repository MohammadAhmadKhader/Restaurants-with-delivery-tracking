using Auth.Contracts.Dtos.Auth;
using Auth.Repositories.IRepositories;
using Microsoft.Extensions.Caching.Hybrid;
using Shared.Redis;

namespace Auth.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly HybridCache _cache;
    private const string Prefix = "tokens:";

    public RefreshTokenRepository(HybridCache cache)
    {
        _cache = cache;
    }

    public async Task StoreRefreshTokenAsync(RefreshToken token)
    {
        var cacheKey = getCacheKey(token.Token);
        var expiry = token.ExpiresAt - DateTime.UtcNow;
        var options = new HybridCacheEntryOptions
        {
            Expiration = expiry
        };

        await _cache.SetAsync(cacheKey, token, options);
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
    {
        var cacheKey = getCacheKey(token);
        var (_, val) = await _cache.TryGetAsync<RefreshToken?>(cacheKey, default);
        return val;
    }

    public async Task<bool> RevokeRefreshTokenAsync(string token)
    {
        var cacheKey = getCacheKey(token);

        try
        {
            await _cache.RemoveAsync(cacheKey);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static string getCacheKey(string token)
    {
        return Prefix + token;
    }
}