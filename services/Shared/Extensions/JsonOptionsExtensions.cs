using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Shared.Common;

namespace Shared.Extensions;
public static class JsonOptionsExtensions
{
    public static IServiceCollection AddNamingPolicy(this IServiceCollection services)
    {
        services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy = AppJsonSerializerOptions.Options.PropertyNamingPolicy;
            options.SerializerOptions.DictionaryKeyPolicy = AppJsonSerializerOptions.Options.DictionaryKeyPolicy;
        });

        return services;
    }
}