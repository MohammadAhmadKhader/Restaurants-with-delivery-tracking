using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

namespace Shared.Redis;

public static class RedisExtensions
{
    public static IServiceCollection AddRedis(
        this IServiceCollection services,
        IConfiguration config,
        string serviceName,
        string connectionString = "Redis")
    {
        var url = config.GetConnectionString(connectionString);
        ArgumentException.ThrowIfNullOrEmpty(url);

        var redisCache = new RedisCache(new RedisCacheOptions() { Configuration = url });
        var serializer = new FusionCacheSystemTextJsonSerializer();
        var defaultFusionEntryOptions = applyDefaultEntryOptions(new FusionCacheEntryOptions());

        services.AddFusionCache(serviceName)
            .WithSerializer(serializer)
            .WithDistributedCache(redisCache)
            .WithOptions(opts =>
            {
                opts.CacheKeyPrefix = serviceName + ":";
                opts.DefaultEntryOptions = defaultFusionEntryOptions;

            })
            .AsHybridCache();

        return services;
    }

    private static readonly Func<FusionCacheEntryOptions, FusionCacheEntryOptions> applyDefaultEntryOptions = opts =>
    {
        var dur = TimeSpan.MaxValue;
        opts.SetSkipMemoryCache(true);
        opts.DistributedCacheDuration = dur;

        return opts;
    };
    
}