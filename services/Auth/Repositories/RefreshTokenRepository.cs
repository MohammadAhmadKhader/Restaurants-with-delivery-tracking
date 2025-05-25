using System.Text.Json;
using Auth.Dtos;
using Auth.Repositories.IRepositories;
using StackExchange.Redis;
using IDatabase = StackExchange.Redis.IDatabase;

namespace Auth.Repositories;
public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly IDatabase _redisDb;
    private const string Prefix = "food_delivery:refresh_tokens:";

    public RefreshTokenRepository(IConnectionMultiplexer redis)
    {
        _redisDb = redis.GetDatabase();
    }

    public async Task StoreRefreshTokenAsync(RefreshToken token)
    {
        var redisKey = Prefix + token.Token;
        var json = JsonSerializer.Serialize(token);
        var expiry = token.ExpiresAt - DateTime.UtcNow;

        await _redisDb.StringSetAsync(redisKey, json, expiry);
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
    {
        var redisKey = Prefix + token;
        var json = await _redisDb.StringGetAsync(redisKey);

        if (json.IsNullOrEmpty)
        {
            return null;
        }

        return JsonSerializer.Deserialize<RefreshToken>(json!);
    }

    public async Task<bool> RevokeRefreshTokenAsync(string token)
    {
        var redisKey = Prefix + token;
        return await _redisDb.KeyDeleteAsync(redisKey);
    }
}