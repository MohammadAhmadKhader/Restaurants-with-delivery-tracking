using System.Net.Http.Headers;
using System.Net.Http.Json;
using Auth.Services.IServices;
using Auth.Tests.Collections;
using Shared.Utils;
using Xunit.Abstractions;

namespace Auth.Tests.Endpoints.Auth;

[Collection("IntegrationTests")]
public class ResetPasswordIntegrationTest(IntegrationTestsFixture fixture, ITestOutputHelper output)
{
    private readonly IntegrationTestsFixture _fixture = fixture;
    private readonly ITestOutputHelper _out = output;
    private readonly int _minPassLength = 6;
    private readonly ITokenService _tokenService = fixture.GetService<ITokenService>();
    private readonly int _maxPassLength = 36;
    private readonly string _endpoint = "api/auth/reset-password";
    private readonly HttpClient _client = fixture.CreateClientWithTestOutput(output);
    
    [Fact]
    public async Task ResetPassword_EmptyOldPassword_ReturnsRequiredError()
    {
        var payload = JsonContent.Create(new
        {
            newPassword = "newPassword123",
            confirmNewPassword = "newPassword123"
        });

        var user = _fixture.Users.ElementAtOrDefault(0);
        var (accessToken, refreshToken) = await TestUtils.Login(_client, user.Email, _fixture.TestPassword);
        var request = TestUtils.GetRequestWithAuth(HttpMethod.Post, _endpoint, accessToken, payload);

        var response = await _client.SendAsync(request);

        await TestUtils.AssertValidationError(response, "oldPassword", "OldPassword is required.");
    }

    [Fact]
    public async Task ResetPassword_ShortOldPassword_ReturnsLengthError()
    {
        var payload = JsonContent.Create(new
        {
            oldPassword = new string('a', _minPassLength - 1),
            newPassword = "newPassword123",
            confirmNewPassword = "newPassword123"
        });

        var user = _fixture.Users.ElementAtOrDefault(0);
        var (accessToken, refreshToken) = await TestUtils.Login(_client, user.Email, _fixture.TestPassword);
        var request = TestUtils.GetRequestWithAuth(HttpMethod.Post, _endpoint, accessToken, payload);

        var response = await _client.SendAsync(request);
        

        await TestUtils.AssertValidationError(response, "oldPassword", "OldPassword must be between 6 and 36 characters.");
    }

    [Fact]
    public async Task ResetPassword_TooLongOldPassword_ReturnsLengthError()
    {
        var payload = JsonContent.Create(new
        {
            oldPassword = new string('x', _maxPassLength + 1),
            newPassword = "newPassword123",
            confirmNewPassword = "newPassword123"
        });

        var user = _fixture.Users.ElementAtOrDefault(0);
        var (accessToken, refreshToken) = await TestUtils.Login(_client, user.Email, _fixture.TestPassword);
        var request = TestUtils.GetRequestWithAuth(HttpMethod.Post, _endpoint, accessToken, payload);

        var response = await _client.SendAsync(request);

        await TestUtils.AssertValidationError(response, "oldPassword", "OldPassword must be between 6 and 36 characters.");
    }

    [Fact]
    public async Task ResetPassword_EmptyNewPassword_ReturnsRequiredError()
    {
        var payload = JsonContent.Create(new
        {
            oldPassword = "oldPassword123",
            confirmNewPassword = "newPassword123"
        });

        var user = _fixture.Users.ElementAtOrDefault(0);
        var (accessToken, refreshToken) = await TestUtils.Login(_client, user.Email, _fixture.TestPassword);
        var request = TestUtils.GetRequestWithAuth(HttpMethod.Post, _endpoint, accessToken, payload);

        var response = await _client.SendAsync(request);

        await TestUtils.AssertValidationError(response, "newPassword", "NewPassword is required.");
    }

    [Fact]
    public async Task ResetPassword_ShortNewPassword_ReturnsLengthError()
    {
        var payload = JsonContent.Create(new
        {
            oldPassword = "oldPassword123",
            newPassword = "123",
            confirmNewPassword = "123"
        });

        var user = _fixture.Users.ElementAtOrDefault(0);
        var (accessToken, refreshToken) = await TestUtils.Login(_client, user.Email, _fixture.TestPassword);
        var request = TestUtils.GetRequestWithAuth(HttpMethod.Post, _endpoint, accessToken, payload);

        var response = await _client.SendAsync(request);

        await TestUtils.AssertValidationError(response, "newPassword", "NewPassword must be between 6 and 36 characters.");
    }

    [Fact]
    public async Task ResetPassword_TooLongNewPassword_ReturnsLengthError()
    {
        var longPass = new string('x', _maxPassLength + 1);
        var payload = JsonContent.Create(new
        {
            oldPassword = "oldPassword123",
            newPassword = longPass,
            confirmNewPassword = longPass
        });

        var user = _fixture.Users.ElementAtOrDefault(0);
        var (accessToken, refreshToken) = await TestUtils.Login(_client, user.Email, _fixture.TestPassword);
        var request = TestUtils.GetRequestWithAuth(HttpMethod.Post, _endpoint, accessToken, payload);

        var response = await _client.SendAsync(request);

        await TestUtils.AssertValidationError(response, "newPassword", "NewPassword must be between 6 and 36 characters.");
    }

    [Fact]
    public async Task ResetPassword_EmptyConfirmPassword_ReturnsRequiredError()
    {
        var payload = JsonContent.Create(new
        {
            oldPassword = "oldPassword123",
            newPassword = "newPassword123"
        });

        var user = _fixture.Users.ElementAtOrDefault(0);
        var (accessToken, refreshToken) = await TestUtils.Login(_client, user.Email, _fixture.TestPassword);
        var request = TestUtils.GetRequestWithAuth(HttpMethod.Post, _endpoint, accessToken, payload);

        var response = await _client.SendAsync(request);

        await TestUtils.AssertValidationError(response, "confirmNewPassword", "ConfirmNewPassword is required.");
    }

    [Fact]
    public async Task ResetPassword_ShortConfirmPassword_ReturnsLengthError()
    {
        var payload = JsonContent.Create(new
        {
            oldPassword = "oldPassword123",
            newPassword = "newPassword123",
            confirmNewPassword = "123"
        });

        var user = _fixture.Users.ElementAtOrDefault(0);
        var (accessToken, refreshToken) = await TestUtils.Login(_client, user.Email, _fixture.TestPassword);
        var request = TestUtils.GetRequestWithAuth(HttpMethod.Post, _endpoint, accessToken, payload);

        var response = await _client.SendAsync(request);

        await TestUtils.AssertValidationError(response, "confirmNewPassword", "ConfirmNewPassword must be between 6 and 36 characters.");
    }

    [Fact]
    public async Task ResetPassword_TooLongConfirmPassword_ReturnsLengthError()
    {
        var longPass = new string('x', _maxPassLength + 1);
        var payload = JsonContent.Create(new
        {
            oldPassword = "oldPassword123",
            newPassword = longPass,
            confirmNewPassword = longPass
        });

        var user = _fixture.Users.ElementAtOrDefault(0);
        var (accessToken, refreshToken) = await TestUtils.Login(_client, user.Email, _fixture.TestPassword);
        var request = TestUtils.GetRequestWithAuth(HttpMethod.Post, _endpoint, accessToken, payload);

        var response = await _client.SendAsync(request);

        await TestUtils.AssertValidationError(response, "confirmNewPassword", "ConfirmNewPassword must be between 6 and 36 characters.");
    }

    [Fact]
    public async Task ResetPassword_PasswordsDoNotMatch_ReturnsMismatchError()
    {
        var payload = JsonContent.Create(new
        {
            oldPassword = "oldPassword123",
            newPassword = "newPassword123",
            confirmNewPassword = "differentPassword123"
        });

        var user = _fixture.Users.ElementAtOrDefault(0);
        var (accessToken, refreshToken) = await TestUtils.Login(_client, user.Email, _fixture.TestPassword);
        var request = TestUtils.GetRequestWithAuth(HttpMethod.Post, _endpoint, accessToken, payload);

        var response = await _client.SendAsync(request);

        await TestUtils.AssertValidationError(response, "confirmNewPassword", "Passwords mismatch.");
    }
}