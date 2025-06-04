using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Auth.Dtos.Auth;
using Auth.Dtos.User;
using Auth.Tests.Collections;
using Auth.Utils;
using Shared.Utils;
using Xunit.Abstractions;

namespace Auth.Tests.Endpoints.Auth;

[Collection("IntegrationTests")]
public class LoginIntegrationTests(IntegrationTestsFixture fixture, ITestOutputHelper output)
{
    private readonly IntegrationTestsFixture _fixture = fixture;
    private readonly ITestOutputHelper _out = output;
    private static readonly string _endpoint = "api/auth/login";
    private readonly HttpClient _client = fixture.CreateClientWithTestOutput(output);

    public static IEnumerable<object[]> InvalidLoginInputs()
    {
        var expectedStatusCode = HttpStatusCode.BadRequest;
        return
        [
            [
                new { email = "email@gmail.com", password = TestUtils.GenerateRandomString(Constants.MinPasswordLength - 1) },
                expectedStatusCode, "password", "Password must be between 6 and 36 characters."
            ],
            [
                new { email = "email@gmail.com", password = TestUtils.GenerateRandomString(Constants.MaxPasswordLength + 1) },
                expectedStatusCode, "password", "Password must be between 6 and 36 characters."
            ],
            [
                new { email = "email@gmail.com", password = "" },
                expectedStatusCode, "password", "Password is required."
            ],
            [
                new { email = "", password = TestUtils.GenerateRandomString(Constants.MinPasswordLength) },
                expectedStatusCode, "email", "Email is required."
            ],
            [
                new { email = "invalid-email", password = TestUtils.GenerateRandomString(Constants.MinPasswordLength) },
                expectedStatusCode, "email", "Invalid email."
            ],
            [
                new { email = new string('a', Constants.MaxEmailLength - "@gmail.com".Length + 1) + "@gmail.com", password = TestUtils.GenerateRandomString(Constants.MinPasswordLength) },
                expectedStatusCode, "email", "Email must be at most 64 characters."
            ]
        ];
    }

    [Theory]
    [MemberData(nameof(InvalidLoginInputs))]
    public async Task Login_InvalidInputs_ReturnsValidationError(object payload, HttpStatusCode expectedStatus, string field, string expectedMessage)
    {
        var response = await _client.PostAsync(_endpoint, JsonContent.Create(payload));
        Assert.Equal(expectedStatus, response.StatusCode);
        await TestUtils.AssertValidationError(response, field, expectedMessage);
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
}