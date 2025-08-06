using Auth.Contracts.Clients;
using Shared.Contracts.Interfaces;
using Shared.Exceptions;
using Shared.Utils;

namespace Shared.Auth;

/// <summary>
/// Provides access to the current user's claims from the Auth microservice.
/// </summary>
/// <remarks>
/// Requires registration via <see cref="HttpExtensions.AddAuthClients(IServiceCollection)" />.
/// Because it uses <see cref="IAuthServiceClient" />.
/// Must be used within an HTTP request after authentication middleware has been applied.
/// </remarks>
public class AuthProvider(
    IAuthServiceClient authServiceClient,
    ITokenProvider tokenProvider
) : IAuthProvider
{
    private IUserInfo? _cachedInfo;
    public IUserInfo UserInfo { get => _cachedInfo ?? throw new InternalServerException("UserInfo is null, must call Auth Api.") ; }
    public async Task<IUserInfo> GetUserInfoAsync()
    {
        if (_cachedInfo != null)
        {
            return _cachedInfo;
        }

        var token = tokenProvider.GetToken();
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new UnauthorizedAccessException("No access token was provided.");
        }

        var result = await RefitUtils.TryCall(authServiceClient.GetUserInfoAsync);
        _cachedInfo = result;

        return _cachedInfo;
    }
}