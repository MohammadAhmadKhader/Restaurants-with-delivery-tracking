using System.Net.Http.Headers;
using Shared.Contracts.Interfaces;

namespace Shared.Auth;

public class AuthenticationClientHandler(ITokenProvider tokenProvider): DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage req, CancellationToken ct)
    {
        var token = tokenProvider.GetToken();
        if (!string.IsNullOrEmpty(token))
        {
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(req, ct);
    }
}