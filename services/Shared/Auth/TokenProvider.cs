using Microsoft.AspNetCore.Http;
using Shared.Contracts.Interfaces;

namespace Shared.Auth;

public class TokenProvider : ITokenProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private string? _cachedToken;
    public TokenProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetToken()
    {
        if (_cachedToken != null)
        {
            return _cachedToken;
        }

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

        _cachedToken = token;
        return _cachedToken;
    }
}