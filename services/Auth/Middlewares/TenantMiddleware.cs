using Auth.Utils;
using Shared.Constants;

namespace Auth.Middlewares;
public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext ctx, ITenantProvider tenantProvider)
    {
        if (ctx.Request.Headers.TryGetValue(CustomHeaders.TenantHeader, out var header) &&
            Guid.TryParse(header[0], out var tenantId))
        {
            tenantProvider.SetTenantId(tenantId);
        }

        await _next(ctx);
    }
}