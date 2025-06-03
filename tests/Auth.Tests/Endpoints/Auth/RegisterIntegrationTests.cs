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
public class RegisterIntegrationTests(IntegrationTestsFixture fixture, ITestOutputHelper output)
{
    private readonly IntegrationTestsFixture _fixture = fixture;
    private readonly ITestOutputHelper _out = output;
    private readonly int _maxPassLength = 36;
    private readonly int _minPassLength = 6;
    private readonly int _maxEmailLength = 64;
    private readonly string _endpoint = "api/auth/register";
    private readonly HttpClient _client = fixture.CreateClientWithTestOutput(output);

    [Fact]
    public async Task Register_EmptyFirstName_ReturnsFirstNameRequiredError()
    {
        var payload = JsonContent.Create(new { lastName = "Doe", email = "john@example.com", password = "password123" });
        var response = await _client.PostAsync(_endpoint, payload);

        await TestUtils.AssertValidationError(response, "firstName", "First name is required.");
    }

    [Fact]
    public async Task Register_ShortFirstName_ReturnsLengthError()
    {
        var payload = JsonContent.Create(new { firstName = "Jo", lastName = "Doe", email = "john@example.com", password = "password123" });
        var response = await _client.PostAsync(_endpoint, payload);

        await TestUtils.AssertValidationError(response, "firstName", "First name must be between 3 and 36 characters.");
    }

    [Fact]
    public async Task Register_TooLongFirstName_ReturnsLengthError()
    {
        var payload = JsonContent.Create(new { firstName = new string('A', 37), lastName = "Doe", email = "john@example.com", password = "password123" });
        var response = await _client.PostAsync(_endpoint, payload);

        await TestUtils.AssertValidationError(response, "firstName", "First name must be between 3 and 36 characters.");
    }

    [Fact]
    public async Task Register_EmptyLastName_ReturnsLastNameRequiredError()
    {
        var payload = JsonContent.Create(new { firstName = "John", email = "john@example.com", password = "password123" });
        var response = await _client.PostAsync(_endpoint, payload);

        await TestUtils.AssertValidationError(response, "lastName", "Last name is required.");
    }

    [Fact]
    public async Task Register_ShortLastName_ReturnsLengthError()
    {
        var payload = JsonContent.Create(new { firstName = "John", lastName = "Do", email = "john@example.com", password = "password123" });
        var response = await _client.PostAsync(_endpoint, payload);

        await TestUtils.AssertValidationError(response, "lastName", "Last name must be between 3 and 36 characters.");
    }

    [Fact]
    public async Task Register_TooLongLastName_ReturnsLengthError()
    {
        var payload = JsonContent.Create(new { firstName = "John", lastName = new string('B', 37), email = "john@example.com", password = "password123" });
        var response = await _client.PostAsync(_endpoint, payload);

        await TestUtils.AssertValidationError(response, "lastName", "Last name must be between 3 and 36 characters.");
    }

    [Fact]
    public async Task Register_EmptyEmail_ReturnsEmailRequiredError()
    {
        var payload = JsonContent.Create(new { firstName = "John", lastName = "Doe", password = "password123" });
        var response = await _client.PostAsync(_endpoint, payload);

        await TestUtils.AssertValidationError(response, "email", "Email is required.");
    }

    [Fact]
    public async Task Register_InvalidEmail_ReturnsInvalidEmailError()
    {
        var payload = JsonContent.Create(new { firstName = "John", lastName = "Doe", email = "invalid", password = "password123" });
        var response = await _client.PostAsync(_endpoint, payload);

        await TestUtils.AssertValidationError(response, "email", "Invalid email.");
    }

    [Fact]
    public async Task Register_TooLongEmail_ReturnsMaxLengthError()
    {
        var longEmail = new string('a', _maxEmailLength - "@gmail.com".Length + 1) + "@gmail.com";
        var payload = JsonContent.Create(new { firstName = "John", lastName = "Doe", email = longEmail, password = "password123" });
        var response = await _client.PostAsync(_endpoint, payload);

        await TestUtils.AssertValidationError(response, "email", "Email must be at most 64 characters.");
    }

    [Fact]
    public async Task Register_EmptyPassword_ReturnsPasswordRequiredError()
    {
        var payload = JsonContent.Create(new { firstName = "John", lastName = "Doe", email = "john@example.com" });
        var response = await _client.PostAsync(_endpoint, payload);

        await TestUtils.AssertValidationError(response, "password", "Password is required.");
    }

    [Fact]
    public async Task Register_ShortPassword_ReturnsLengthError()
    {
        var payload = JsonContent.Create(new { firstName = "John", lastName = "Doe", email = "john@example.com", password = "123" });
        var response = await _client.PostAsync(_endpoint, payload);

        await TestUtils.AssertValidationError(response, "password", "Password must be between 6 and 36 characters.");
    }

    [Fact]
    public async Task Register_TooLongPassword_ReturnsLengthError()
    {
        var payload = JsonContent.Create(new { firstName = "John", lastName = "Doe", email = "john@example.com", password = new string('x', _maxPassLength + 1) });
        var response = await _client.PostAsync(_endpoint, payload);

        await TestUtils.AssertValidationError(response, "password", "Password must be between 6 and 36 characters.");
    }

    [Fact]
    public async Task Register_WithAlreadytExistedUser_ReturnsUserAlreadyExists()
    {
        var email = TestDataLoader.UserEmail;
        var payload = JsonContent.Create(new { firstName = "john", lastName = "doe", email, password = TestDataLoader.TestPassword });
        var response = await _client.PostAsync(_endpoint, payload);
        var body = await response.Content.ReadFromJsonAsync<Dictionary<string, JsonElement>>();

        Assert.NotNull(body);
        body.TryGetValue("detail", out var detailElm);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal($"Duplicate email '{email}' detected.", detailElm.GetString());
    }

    [Fact]
    public async Task Register_CorrectRegister_ReturnsInvalidEmailOrPassword()
    {
        var payload = JsonContent.Create(new { firstName = "john", lastName = "doe", email = "newUser11111@gmail.com", password = TestDataLoader.TestPassword });
        var response = await _client.PostAsync(_endpoint, payload);
        var body = await response.Content.ReadFromJsonAsync<UserWithTokensDto>();

        Assert.NotNull(body);
        _out.WriteLine(await response.Content.ReadAsStringAsync());
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        Assert.IsType<UserWithRolesAndPermissionsDto>(body.User);
        Assert.False(string.IsNullOrWhiteSpace(body.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(body.RefreshToken));
    }
}