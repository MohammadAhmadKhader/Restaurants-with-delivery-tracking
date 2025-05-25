using System.Text.Json;
using Auth.Dtos;
using Auth.Repositories.IRepositories;
using StackExchange.Redis;

namespace Auth.Services;
public class RedisService : IRedisService
{
    private readonly IDatabase _redisDb;
    private readonly ILogger<RedisService> _logger;

    public RedisService(IConnectionMultiplexer redis, ILogger<RedisService> logger)
    {
        _redisDb = redis.GetDatabase();
        _logger = logger;
    }

    public async Task SetStringAsync(string key, string value)
    {
        await _redisDb.StringSetAsync(key, value);
    }

    public async Task<string?> GetStringAsync(string key)
    {
        return await _redisDb.StringGetAsync(key);
    }

    public async Task<TData?> GetAsync<TData>(string key)
    {
        var json = await _redisDb.StringGetAsync(key);

        if (json.IsNullOrEmpty)
        {
            _logger.LogInformation("Key {key} not found or empty.", key);
            return default;
        }

        return JsonSerializer.Deserialize<TData>(json!);
    }
}