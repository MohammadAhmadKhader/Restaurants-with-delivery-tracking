using Payments.Config;

namespace Payments.Extensions;
public static class StripeExtensions
{
    public static IServiceCollection AddStripeConfig(this IServiceCollection services, IConfiguration config)
    {
        services
            .AddOptions<StripeConfig>()
            .Bind(config.GetSection(StripeConfig.sectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
    
        return services;
    }
}