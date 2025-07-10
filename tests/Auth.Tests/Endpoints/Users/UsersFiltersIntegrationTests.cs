using System.Text.Json;
using Auth.Models;
using Auth.Tests.Collections;

namespace Auth.Tests.Endpoints.Users;

[Collection("IntegrationTests")]
public class UsersFiltersIntegrationTests(IntegrationTestsFixture fixture, ITestOutputHelper output)
{
    private readonly ITestOutputHelper _out = output;
    private readonly IntegrationTestsFixture _fixture = fixture;
    private readonly string _endpoint = "api/users";
    private readonly HttpClient _client = fixture.CreateClientWithTestOutput(output);
    private const string collectionKey = "items";

    [Theory]
    [InlineData("SuperAdmin")]
    [InlineData("Admin")]
    public async Task FilterUsers_WithDifferentAdminRoles_ReturnsFilteredUsers(string role)
    {
        TestUtils.LogPayload(_out, [new { role }]);
        var queryParams = new Dictionary<string, string>{};
        var queryString = TestUtils.GetQueryString(queryParams);

        var user = role switch
        {
            "SuperAdmin" => _fixture.GetSuperAdmin(),
            "Admin" => _fixture.GetAdmin(),
            _ => throw new ArgumentException("Unsupported role")
        };
        Assert.NotNull(user);

        var (accessToken, _) = await TestUtils.Login(_client, user.Email!, _fixture.TestPassword);
        var request = TestUtils.GetRequestWithAuth(HttpMethod.Get, $"{_endpoint}?{queryString}", accessToken, null);

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        
        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);

        if (!doc.RootElement.TryGetProperty(collectionKey, out var usersJsonElement))
        {
            Assert.Fail("users were not returned");
        }

        var users = TestUtils.Deserialize<List<User>>(usersJsonElement);
        Assert.NotNull(users);
        Assert.NotEmpty(users);
    }

    [Fact]
    public async Task FilterUsers_WithForbiddenRole_ReturnsFilteredUsers()
    {
        var queryParams = new Dictionary<string, string> { };
        var queryString = TestUtils.GetQueryString(queryParams);

        var user = _fixture.Loader.Users.FirstOrDefault(u => u.Email == TestDataLoader.UserEmail);
        Assert.NotNull(user);

        var (accessToken, _) = await TestUtils.Login(_client, user.Email!, _fixture.TestPassword);
        var request = TestUtils.GetRequestWithAuth(HttpMethod.Get, $"{_endpoint}?{queryString}", accessToken, null);

        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        Assert.True(body == "" || body == null);
    }
    
    [Theory]
    [InlineData("firstName", "John")]
    [InlineData("lastName", "doe")]
    [InlineData("email", "superAdmin@gmail.com")]
    public async Task FilterUsers_ByField_ReturnsFilteredUsers(string fieldName, string expectedValue)
    {
        TestUtils.LogPayload(_out, [new { fieldName, expectedValue }]);
        var queryParams = new Dictionary<string, string>
        {
            { fieldName, expectedValue }
        };
        var queryString = TestUtils.GetQueryString(queryParams);

        var user = _fixture.GetSuperAdmin();
        Assert.NotNull(user);

        var (accessToken, _) = await TestUtils.Login(_client, user.Email!, _fixture.TestPassword);
        var request = TestUtils.GetRequestWithAuth(HttpMethod.Get, $"{_endpoint}?{queryString}", accessToken, null);

        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        
        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);

        if (!doc.RootElement.TryGetProperty(collectionKey, out var usersJsonElement))
        {
            Assert.Fail("users were not returned");
        }

        var users = TestUtils.Deserialize<List<User>>(usersJsonElement);
        Assert.NotNull(users);
        Assert.NotEmpty(users);

        foreach (var u in users)
        {
            var actualValue = fieldName switch
            {
                "firstName" => u.FirstName,
                "lastName" => u.LastName,
                "email" => u.Email,
                _ => throw new NotSupportedException($"Filtering by {fieldName} is not supported.")
            };

            Assert.Equal(expectedValue, actualValue, ignoreCase: true);
        }
    }

    [Fact]
    public async Task FilterUsers_FilterByCreatedAfter_ReturnsUsersCreatedAfter()
    {
        var date = DateTime.UtcNow.AddDays(-1).ToString("o");
        var queryParams = new Dictionary<string, string>
        {
            { "createdAfter", date },
        };
        var queryString = TestUtils.GetQueryString(queryParams);

        var user = _fixture.GetSuperAdmin();
        Assert.NotNull(user);

        var (accessToken, refreshToken) = await TestUtils.Login(_client, user.Email!, _fixture.TestPassword);
        var request = TestUtils.GetRequestWithAuth(HttpMethod.Get, $"{_endpoint}?{queryString}", accessToken, null);
        var response = await _client.SendAsync(request);

        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);

        if (!doc.RootElement.TryGetProperty(collectionKey, out var usersJsonElement))
        {
            Assert.Fail("users were not returned");
        }

        var users = TestUtils.Deserialize<List<User>>(usersJsonElement);
        Assert.NotNull(users);

        foreach (var u in users)
        {
            Assert.True(u.CreatedAt >= DateTime.Parse(date));
        }
    }

    [Fact]
    public async Task FilterUsers_FilterByCreatedBefore_ReturnsUsersCreatedBefore()
    {
        var date = DateTime.UtcNow.AddDays(1).ToString("o");
        var queryParams = new Dictionary<string, string>
        {
            { "createdBefore", date },
        };
        var queryString = TestUtils.GetQueryString(queryParams);

        var user = _fixture.GetSuperAdmin();
        Assert.NotNull(user);

        var (accessToken, refreshToken) = await TestUtils.Login(_client, user.Email!, _fixture.TestPassword);
        var request = TestUtils.GetRequestWithAuth(HttpMethod.Get, $"{_endpoint}?{queryString}", accessToken, null);
        var response = await _client.SendAsync(request);

        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);

        if (!doc.RootElement.TryGetProperty(collectionKey, out var usersJsonElement))
        {
            Assert.Fail("users were not returned");
        }

        var users = TestUtils.Deserialize<List<User>>(usersJsonElement);
        Assert.NotNull(users);

        foreach (var u in users)
        {
            Assert.True(u.CreatedAt <= DateTime.Parse(date));
        }
    }
    
    [Theory]
    [InlineData("firstName")]
    [InlineData("lastName")]
    [InlineData("createdAt")]
    public async Task FilterUsers_SortByFieldAscending_ReturnsUsersSortedByField(string field)
    {
        TestUtils.LogPayload(_out, [new { field }]);
        var queryParams = new Dictionary<string, string>
        {
            { "sortField", field },
            { "sortDir", "asc" },
        };
        var queryString = TestUtils.GetQueryString(queryParams);
        TestUtils.LogPayload(_out, [new { queryString }]);

        var user = _fixture.GetSuperAdmin();
        Assert.NotNull(user);

        var (accessToken, refreshToken) = await TestUtils.Login(_client, user.Email!, _fixture.TestPassword);
        var request = TestUtils.GetRequestWithAuth(HttpMethod.Get, $"{_endpoint}?{queryString}", accessToken, null);
        var response = await _client.SendAsync(request);

        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        if (!doc.RootElement.TryGetProperty(collectionKey, out var usersJsonElement))
        {
            Assert.Fail("users were not returned");
        }

        var users = TestUtils.Deserialize<List<User>>(usersJsonElement);
        Assert.NotNull(users);

        var property = TestUtils.GetProperty<User>(field);
        Assert.NotNull(property);

        var sorted = users.OrderByDescending(u => property.GetValue(u)).ToList();
        Assert.True(users.SequenceEqual(sorted));
    }

    [Theory]
    [InlineData("firstName")]
    [InlineData("lastName")]
    [InlineData("createdAt")]
    public async Task FilterUsers_SortByFieldDescending_ReturnsUsersSortedByField(string field)
    {
        TestUtils.LogPayload(_out, [new { field }]);
        var queryParams = new Dictionary<string, string>
        {
            { "sortField", field },
            { "sortDir", "desc" },
        };
        var queryString = TestUtils.GetQueryString(queryParams);
        TestUtils.LogPayload(_out, [new { queryString }]);

        var user = _fixture.GetSuperAdmin();
        Assert.NotNull(user);

        var (accessToken, refreshToken) = await TestUtils.Login(_client, user.Email!, _fixture.TestPassword);
        var request = TestUtils.GetRequestWithAuth(HttpMethod.Get, $"{_endpoint}?{queryString}", accessToken, null);
        var response = await _client.SendAsync(request);

        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        if (!doc.RootElement.TryGetProperty(collectionKey, out var usersJsonElement))
        {
            Assert.Fail("users were not returned");
        }

        var users = TestUtils.Deserialize<List<User>>(usersJsonElement);
        Assert.NotNull(users);
        
        var property = TestUtils.GetProperty<User>(field);
        Assert.NotNull(property);

        var sorted = users.OrderByDescending(u => property.GetValue(u)).ToList();
        Assert.True(users.SequenceEqual(sorted));
    }

    [Fact]
    public async Task FilterUsers_CombinedFilters_ReturnsCorrectUsers()
    {
        var queryParams = new Dictionary<string, string>
        {
            { "firstName", "john" },
            { "lastName", "doe" },
            { "email", "superAdmin@gmail.com" }
        };
        var queryString = TestUtils.GetQueryString(queryParams);

        var user = _fixture.GetSuperAdmin();
        Assert.NotNull(user);

        var (accessToken, refreshToken) = await TestUtils.Login(_client, user.Email!, _fixture.TestPassword);
        var request = TestUtils.GetRequestWithAuth(HttpMethod.Get, $"{_endpoint}?{queryString}", accessToken, null);
        var response = await _client.SendAsync(request);

        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        if (!doc.RootElement.TryGetProperty(collectionKey, out var usersJsonElement))
        {
            Assert.Fail("users were not returned");
        }

        var users = TestUtils.Deserialize<List<User>>(usersJsonElement);
        Assert.NotNull(users);
        foreach (var u in users)
        {
            Assert.Equal("john", u.FirstName, ignoreCase: true);
            Assert.Equal("doe", u.LastName, ignoreCase: true);
            Assert.Equal("superAdmin@gmail.com", u.Email, ignoreCase: true);
        }
        Assert.NotEmpty(users);
    }
}