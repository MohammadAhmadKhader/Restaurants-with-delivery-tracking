using Auth.Contracts.Clients;
using Auth.Contracts.Dtos.Auth;
using Shared.Config;
using Shared.Contracts.Interfaces;

namespace Shared.Http.Clients;

public class AuthServiceClient : BaseServiceClient, IAuthServiceClient
{
    private const string baseEndpoint = "/api/auth";
    public AuthServiceClient(IHttpClientService httpClientService) : base(httpClientService, MicroservicesUrlsProvider.Config.AuthService)
    {

    }
    public async Task<UserWithTokensDto> LoginAsync(LoginDto dto)
    {
        var response = await _httpClientService.PostAsync<LoginDto, UserWithTokensDto>(_baseUrl + baseEndpoint + "/login", dto);
        return response!;
    }

    public async Task<UserWithTokensDto> RegisterAsync(RegisterDto dto)
    {
        var response = await _httpClientService.PostAsync<RegisterDto, UserWithTokensDto>(_baseUrl + baseEndpoint + "/register", dto);
        return response!;
    }

    public async Task<object> ResetPasswordAsync(ResetPasswordDto dto)
    {
        var response = await _httpClientService.PostAsync<ResetPasswordDto, object>(_baseUrl + baseEndpoint + "/register", dto);
        return response!;
    }

    public Task<object> TestAsync(object data)
    {
        throw new NotImplementedException();
    }
}