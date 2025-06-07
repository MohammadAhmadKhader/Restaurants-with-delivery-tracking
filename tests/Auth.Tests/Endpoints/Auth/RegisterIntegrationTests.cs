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
public class RegisterIntegrationTests(IntegrationTestsFixture fixture, ITestOutputHelper output)
{
    private readonly IntegrationTestsFixture _fixture = fixture;
    private readonly ITestOutputHelper _out = output;
    private readonly string _endpoint = "api/auth/register";
    private readonly HttpClient _client = fixture.CreateClientWithTestOutput(output);

    public static IEnumerable<object[]> InvalidRegisterInputs()
    {
        const HttpStatusCode expectedStatusCode = HttpStatusCode.BadRequest;
        const string firstName = "firstName";
        const string lastName = "lastName";
        const string email = "email";
        const string password = "password";
        var longEmail = new string('a', Constants.MaxEmailLength - "@gmail.com".Length + 1) + "@gmail.com";
        var longFirstName = new string('A', Constants.MaxFirstNameLength + 1);
        var longLastName = new string('B', Constants.MaxLastNameLength + 1);
        var longPassword = new string('x', Constants.MaxPasswordLength + 1);
        var shortPassword = new string('x', Constants.MinPasswordLength - 1);
        var shortFirstName = new string('x', Constants.MinFirstNameLength - 1);
        var shortLastName = new string('x', Constants.MinLastNameLength - 1);
        return
        [
            // firstName
            [
                new { lastName = "Doe", email = "john@example.com", password = "password123" },
                expectedStatusCode, firstName, "FirstName is required."
            ],
            [
                new { firstName = shortFirstName, lastName = "Doe", email = "john@example.com", password = "password123" },
                expectedStatusCode, firstName, "FirstName must be between 3 and 36 characters."
            ],
            [
                new { firstName = longFirstName, lastName = "Doe", email = "john@example.com", password = "password123" },
                expectedStatusCode, firstName, "FirstName must be between 3 and 36 characters."
            ],
            // lastName
            [
                new { firstName = "John", email = "john@example.com", password = "password123" },
                expectedStatusCode, lastName, "LastName is required."
            ],
            [
                new { firstName = "John", lastName = shortLastName, email = "john@example.com", password = "password123" },
                expectedStatusCode, lastName, "LastName must be between 3 and 36 characters."
            ],
            [
                new { firstName = "John", lastName = longLastName, email = "john@example.com", password = "password123" },
                expectedStatusCode, lastName, "LastName must be between 3 and 36 characters."
            ],
            // email
            [
                new { firstName = "John", lastName = "Doe", password = "password123" },
                expectedStatusCode, email, "Email is required."
            ],
            [
                new { firstName = "John", lastName = "Doe", email = "invalid", password = "password123" },
                expectedStatusCode, email, "Invalid email."
            ],
            [
                new { firstName = "John", lastName = "Doe", email = longEmail, password = "password123" },
                expectedStatusCode, email, "Email must be at most 64 characters."
            ],
            // password
            [
                new { firstName = "John", lastName = "Doe", email = "john@example.com" },
                expectedStatusCode, password, "Password is required."
            ],
            [
                new { firstName = "John", lastName = "Doe", email = "john@example.com", password = shortPassword },
                expectedStatusCode, password, "Password must be between 6 and 36 characters."
            ],
            [
                new { firstName = "John", lastName = "Doe", email = "john@example.com", password = longPassword },
                expectedStatusCode, password, "Password must be between 6 and 36 characters."
            ]
        ];
    }

    [Theory]
    [MemberData(nameof(InvalidRegisterInputs))]
    public async Task Register_InvalidInputs_ReturnsValidationError(object payload, HttpStatusCode expectedStatus, string field, string expectedMessage)
    {
        var response = await _client.PostAsync(_endpoint, JsonContent.Create(payload));
        Assert.Equal(expectedStatus, response.StatusCode);
        await TestUtils.AssertValidationError(response, field, expectedMessage);
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
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        Assert.IsType<UserWithRolesAndPermissionsDto>(body.User);
        Assert.False(string.IsNullOrWhiteSpace(body.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(body.RefreshToken));
    }
}