using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shared.Constants;

namespace Shared.Tenant;
public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext ctx, ITenantProvider tenantProvider, ILogger<TenantMiddleware> logger)
    {
        if (ctx.Request.Headers.TryGetValue(CustomHeaders.TenantHeader, out var header) &&
            Guid.TryParse(header[0], out var tenantId))
        {
            tenantProvider.SetTenantId(tenantId);
            logger.LogInformation("TenantId is '{TenantId}'", tenantId);
        }
        else
        {
            logger.LogInformation("TenantId is '{TenantId}'", "null");
        }

        await _next(ctx);
    }
}