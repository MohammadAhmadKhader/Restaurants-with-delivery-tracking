using Notifications.Config;

namespace Notifications.Extensions;
public static class ConfigsExtensions
{
    public static IServiceCollection AddConfigs(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<EmailSettings>()
            .Bind(config.GetSection(EmailSettings.sectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<EmailUrlsSettings>()
            .Bind(config.GetSection(EmailUrlsSettings.sectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
    
        return services;
    }
}