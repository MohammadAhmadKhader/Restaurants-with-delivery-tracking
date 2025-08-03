using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Utils;

namespace Shared.Storage.Cloudinary;

public static class CloudinaryExtensions
{
    public static IServiceCollection AddCloudinaryStorage(
        this IServiceCollection services,
        IConfiguration config,
        string sectionName = "Cloudinary")
    {
        services.AddOptions<CloudinarySettings>()
            .Bind(config.GetSection(sectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IFileStorageService, CloudinaryFileStorageService>(sp =>
        {
            var opts = sp.GetRequiredService<IOptions<CloudinarySettings>>();
            var logger  = sp.GetRequiredService<ILogger<CloudinaryFileStorageService>>();
            return new CloudinaryFileStorageService(opts, logger, GeneralUtils.GetServiceName());
        });

        return services;
    }
}