using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Shared.Constants;
using Shared.Contracts.Interfaces;
using Shared.Utils;

namespace Shared.Auth;

public class AuthenticationClientHandler(
    ITokenProvider tokenProvider,
    IHttpContextAccessor httpContextAccessor
): DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage req, CancellationToken ct)
    {
        var token = tokenProvider.GetToken();
        if (!string.IsNullOrEmpty(token))
        {
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var ctx = httpContextAccessor.HttpContext;
        GuardUtils.ThrowIfNull(ctx);
        
        if (ctx.Request.Headers.TryGetValue(CustomHeaders.TenantHeader, out var restaurantId))
        {
            req.Headers.Add(CustomHeaders.TenantHeader, (string) restaurantId!);
        }

        return await base.SendAsync(req, ct);
    }
}