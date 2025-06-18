using Microsoft.Extensions.Configuration;

namespace Shared.Config;

public static class MicroservicesUrlsProvider
{
    public readonly static MicroservicesUrlsOptions Config = LoadConfiguration();
    public static MicroservicesUrlsOptions LoadConfiguration()
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("MicroservicesUrls.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"MicroservicesUrls.{env}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var microservicesUrls = config.GetSection("MicroservicesUrls").Get<MicroservicesUrlsOptions>();
        if (microservicesUrls == null)
        {
            throw new InvalidOperationException("Missing MicroservicesUrls config");
        }

        return microservicesUrls;
    }
}

public class MicroservicesUrlsOptions
{
    public string AuthService { get; set; } = default!;
    public string OrdersService { get; set; } = default!;
    public string PaymentsService { get; set; } = default!;
    public string LocationsService { get; set; } = default!;
    public string RestaurantsService { get; set; } = default!;
    public string ReviewsService { get; set; } = default!;
}
