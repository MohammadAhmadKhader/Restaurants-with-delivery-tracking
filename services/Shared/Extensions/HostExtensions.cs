using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace Shared.Extensions;
public static class HostExtensions
{
    public static ConfigureHostBuilder ValidateScopes(this ConfigureHostBuilder host)
    {
        host.UseDefaultServiceProvider((ctx, opts) =>
        {
            opts.ValidateScopes = true;
            opts.ValidateOnBuild = true;
        });
        return host;
    }
}