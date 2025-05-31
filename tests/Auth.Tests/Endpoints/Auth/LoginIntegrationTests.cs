using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
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
}