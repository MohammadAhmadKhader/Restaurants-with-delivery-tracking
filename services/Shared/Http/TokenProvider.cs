using Microsoft.AspNetCore.Http;
using Shared.Contracts.Interfaces;

namespace Shared.Http;

public class TokenProvider : ITokenProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public TokenProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetToken()
    {
        var ctx = _httpContextAccessor.HttpContext;
        if (ctx == null)
        {
            throw new ArgumentNullException("HttpContextAccessor is not activated");
        }

        var authHeader = ctx.Request.Headers.Authorization.ToString();
        string? token = null;

        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            token = authHeader.Substring("Bearer ".Length);
        }

        return token;
    }

    public string? GetAuthorizationHeader()
    {
        var ctx = _httpContextAccessor.HttpContext;
        if (ctx == null)
        {
            throw new ArgumentNullException("HttpContextAccessor is not activated");
        }

        return ctx.Request.Headers.Authorization.ToString();
    }
}