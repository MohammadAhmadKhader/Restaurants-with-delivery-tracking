using Auth.Utils;

namespace Auth.Middlewares;
public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantProvider tenantProvider)
    {
        if (context.Request.Headers.TryGetValue(TenantProvider.TenantHeader, out var header) &&
            Guid.TryParse(header.FirstOrDefault(), out var tenantId))
        {
            tenantProvider.SetTenantId(tenantId);
        }

        await _next(context);
    }
}