using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Auth.Dtos.Auth;
using Auth.Dtos.User;
using Auth.Tests.Collections;
using Shared.Utils;
using Xunit.Abstractions;

namespace Auth.Tests.Endpoints.Auth;

[Collection("IntegrationTests")]
public class LoginIntegrationTests(IntegrationTestsFixture fixture, ITestOutputHelper output)
{
    private readonly IntegrationTestsFixture _fixture = fixture;
    private readonly ITestOutputHelper _out = output;
    private readonly int _maxPassLength = 36;
    private readonly int _minPassLength = 6;
    private readonly int _maxEmailLength = 64;
    private readonly string _endpoint = "api/auth/login";
    private readonly HttpClient _client = fixture.CreateClientWithTestOutput(output);

    [Fact]
    public async Task Login_ShortPassword_ReturnsBadRequest()
    {
        var payload = JsonContent.Create(new { email = "email@gmail.com", password = TestUtils.GenerateRandomString(_minPassLength - 1) });
        var response = await _client.PostAsync(_endpoint, payload);

        await TestUtils.AssertValidationError(response, "password", "Password must be between 6 and 36 characters.");
    }

    [Fact]
    public async Task Login_LongPassword_ReturnsBadRequest()
    {
        var payload = JsonContent.Create(new { email = "email@gmail.com", password = TestUtils.GenerateRandomString(_maxPassLength + 1) });
        var response = await _client.PostAsync(_endpoint, payload);

        await TestUtils.AssertValidationError(response, "password", "Password must be between 6 and 36 characters.");
    }

    [Fact]
    public async Task Login_EmptyPassword_ReturnsBadRequest()
    {
        var payload = JsonContent.Create(new { email = "email@gmail.com", password = "" });
        var response = await _client.PostAsync(_endpoint, payload);

        await TestUtils.AssertValidationError(response, "password", "Password is required.");
    }

    [Fact]
    public async Task Login_EmptyEmail_ReturnsBadRequest()
    {
        var payload = JsonContent.Create(new { email = "", password = TestUtils.GenerateRandomString(_minPassLength) });
        var response = await _client.PostAsync(_endpoint, payload);

        await TestUtils.AssertValidationError(response, "email", "Email is required.");
    }

    [Fact]
    public async Task Login_InvalidEmail_ReturnsBadRequest()
    {
        var payload = JsonContent.Create(new { email = "invalid-email", password = TestUtils.GenerateRandomString(_minPassLength) });
        var response = await _client.PostAsync(_endpoint, payload);

        await TestUtils.AssertValidationError(response, "email", "Invalid email.");
    }

    [Fact]
    public async Task Login_TooLongEmail_ReturnsBadRequest()
    {
        var longEmail = new string('a', _maxEmailLength - "@gmail.com".Length + 1) + "@gmail.com";
        var payload = JsonContent.Create(new { email = longEmail, password = TestUtils.GenerateRandomString(_minPassLength) });
        var response = await _client.PostAsync(_endpoint, payload);

        await TestUtils.AssertValidationError(response, "email", "Email must be at most 64 characters.");
    }

    [Fact]
    public async Task Login_CorrectLogin_ReturnsOkWithUsersAndTokens()
    {
        var payload = JsonContent.Create(new { email = TestDataLoader.UserEmail, password = TestDataLoader.TestPassword });
        var response = await _client.PostAsync(_endpoint, payload);
        var body = await response.Content.ReadFromJsonAsync<UserWithTokensDto>();

        Assert.NotNull(body);
        Assert.IsType<UserWithRolesAndPermissionsDto>(body.User);
        Assert.False(string.IsNullOrWhiteSpace(body.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(body.RefreshToken));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsInvalidEmailOrPassword()
    {
        var payload = JsonContent.Create(new { email = TestDataLoader.UserEmail, password = "2342423f23" });
        var response = await _client.PostAsync(_endpoint, payload);
        var body = await response.Content.ReadFromJsonAsync<Dictionary<string, JsonElement>>();

        Assert.NotNull(body);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        body.TryGetValue("detail", out var detailElm);
        var errMsg = detailElm.GetString();

        Assert.Equal("Invalid email or password.", errMsg);
    }

    [Fact]
    public async Task Login_UserDoesNotExist_ReturnsInvalidEmailOrPassword()
    {
        var payload = JsonContent.Create(new { email = "notExistentUser@gmail.com", password = TestDataLoader.TestPassword });
        var response = await _client.PostAsync(_endpoint, payload);
        var body = await response.Content.ReadFromJsonAsync<Dictionary<string, JsonElement>>();

        Assert.NotNull(body);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        body.TryGetValue("detail", out var detailElm);
        var errMsg = detailElm.GetString();

        Assert.Equal("Invalid email or password.", errMsg);
    }
}