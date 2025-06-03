using System.Net;
using System.Net.Http.Json;
using Auth.Models;
using Auth.Repositories.IRepositories;
using Auth.Tests.Collections;
using Microsoft.Extensions.DependencyInjection;
using Shared.Utils;
using Xunit.Abstractions;

namespace Auth.Tests.Endpoints.Roles;

[Collection("IntegrationTests")]
public class RoleIntegrationTests
{
    private static IntegrationTestsFixture _fixture;
    private readonly ITestOutputHelper _out;

    private static readonly int _minLength = 3;
    private static readonly int _maxLength = 36;
    private static readonly string _mainEndpoint = "api/roles";
    private readonly HttpClient _client;
    private static Guid _roleIdToUpdate;

    public RoleIntegrationTests(IntegrationTestsFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _out = output;
        _client = fixture.CreateClientWithTestOutput(output);

        InitializeRoleToUpdate();
    }

    private void InitializeRoleToUpdate()
    {
        var roleToUpdate = new Role
        {
            Name = "NewRole",
            DisplayName = "NewRole"
        };

        Task.Run(async () =>
        {
            using var scope = _fixture.Factory.Services.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            await unitOfWork.RolesRepository.CreateAsync(roleToUpdate);
            await unitOfWork.SaveChangesAsync();

            _roleIdToUpdate = roleToUpdate.Id;
        }).GetAwaiter().GetResult();
    }

    #region Get Roles (Pagination)

    [Theory]
    [ClassData(typeof(PaginationTestData))]
    public async Task GetRoles_PaginationTests(int? page, int? size, int expectedPage, int expectedSize)
    {
        var user = _fixture.GetSuperAdmin();
        Assert.NotNull(user);

        await PaginationTestUtils.TestPagination(_client, (user.Email, _fixture.TestPassword)!, _mainEndpoint, page, size, expectedPage, expectedSize);
    }

    #endregion

    #region Create Role

    public static IEnumerable<object[]> CreateRoleInvalidInputs => new List<object[]>
    {
        new object[] { new Dictionary<string, object> { ["name"] = null, ["displayName"] = "DisplayName" },
            "name", "Name is required." },
        new object[] { new Dictionary<string, object> { ["name"] = "Name", ["displayName"] = null },
            "displayName", "Display name is required." },
        new object[] { new Dictionary<string, object> { ["name"] = new string('a', _minLength - 1) },
            "name", "Name must be between 3 and 36 characters." },
        new object[] { new Dictionary<string, object> { ["name"] = new string('a', _minLength), ["displayName"] = new string('a', _minLength - 1) },
            "displayName", "Display name must be between 3 and 36 characters." },
        new object[] { new Dictionary<string, object> { ["name"] = new string('x', _maxLength + 1) },
            "name", "Name must be between 3 and 36 characters." },
        new object[] { new Dictionary<string, object> { ["name"] = new string('x', _maxLength) ,["displayName"] = new string('x', _maxLength + 1) },
            "displayName", "Display name must be between 3 and 36 characters." },
    };

    [Theory]
    [MemberData(nameof(CreateRoleInvalidInputs))]
    public async Task CreateRole_InvalidInputas_ReturnsValidationError(Dictionary<string, object> payloadDict, string field, string expectedMessage)
    {
        var jsonPayload = JsonContent.Create(payloadDict);

        var user = _fixture.GetSuperAdmin();
        Assert.NotNull(user);

        var role = _fixture.Roles.FirstOrDefault();
        Assert.NotNull(role);

        var (accessToken, _) = await TestUtils.Login(_client, user.Email!, _fixture.TestPassword);
        var request = TestUtils.GetRequestWithAuth(HttpMethod.Post, $"{_mainEndpoint}", accessToken, jsonPayload);

        var response = await _client.SendAsync(request);

        await TestUtils.AssertValidationError(response, field, expectedMessage);
    }

    [Fact]
    public async Task CreateRole_ValidData_ReturnsSuccess()
    {
        var payload = JsonContent.Create(new
        {
            displayName = "Test Role",
            name = "test-role"
        });

        var user = _fixture.GetSuperAdmin();
        Assert.NotNull(user);

        var (accessToken, refreshToken) = await TestUtils.Login(_client, user.Email!, _fixture.TestPassword);
        var request = TestUtils.GetRequestWithAuth(HttpMethod.Post, _mainEndpoint, accessToken, payload);

        var response = await _client.SendAsync(request);

        Assert.True(response.IsSuccessStatusCode, $"Expected success but got {response.StatusCode}");
    }

    #endregion

    #region Update Role

    public static IEnumerable<object[]> UpdateRoleInvalidInputs => new List<object[]>
    {
        new object[] { "displayName", new { displayName = new string('a', _minLength - 1) }, "Display name must be between 3 and 36 characters." },
        new object[] { "displayName", new { displayName =  new string('x', _maxLength + 1) }, "Display name must be between 3 and 36 characters." },
        new object[] { "name", new { name = new string('a', _minLength - 1) }, "Name must be between 3 and 36 characters." },
        new object[] { "name", new { name = new string('x', _maxLength + 1)}, "Name must be between 3 and 36 characters." },
        new object[] { "role", new { displayName = "", name = "" }, "At least one of 'Name' or 'DisplayName' must be provided." },
        new object[] { "role", new { displayName = "   ", name = "   " }, "At least one of 'Name' or 'DisplayName' must be provided." },
        new object[] { "role", new { displayName = (string?)null, name = (string?)null }, "At least one of 'Name' or 'DisplayName' must be provided." },
    };

    [Theory]
    [MemberData(nameof(UpdateRoleInvalidInputs))]
    public async Task UpdateRole_InvalidInputs_ReturnsValidationError(string fieldName, object payload, string expectedError)
    {
        var roleId = Guid.NewGuid();
        var user = _fixture.GetSuperAdmin();
        Assert.NotNull(user);

        var (accessToken, refreshToken) = await TestUtils.Login(_client, user.Email!, _fixture.TestPassword);
        var jsonPayload = JsonContent.Create(payload);

        var request = TestUtils.GetRequestWithAuth(HttpMethod.Put, $"{_mainEndpoint}/{roleId}", accessToken, jsonPayload);
        var response = await _client.SendAsync(request);

        await TestUtils.AssertValidationError(response, fieldName, expectedError);
    }

    [Theory]
    [InlineData(new string[] { "name" }, new string[] { "Updated Role Namex1" })]
    [InlineData(new string[] { "displayName" }, new string[] { "Updated Role Display Namex2" })]
    [InlineData(new string[] { "name", "displayName" }, new string[] { "Updated Role Namex3", "Updated Role Display Namex3" })]
    public async Task UpdateRole_ValidInputs_ReturnsSuccess(string[] fieldName, string[] fieldValue)
    {
        var user = _fixture.GetSuperAdmin();
        Assert.NotNull(user);

        var (accessToken, _) = await TestUtils.Login(_client, user.Email!, _fixture.TestPassword);

        var payload = new Dictionary<string, string>();
        for (int i = 0; i < fieldName.Length; i++)
        {
            payload[fieldName[i]] = fieldValue[i];
        }
        var jsonContent = JsonContent.Create(payload);
        var request = TestUtils.GetRequestWithAuth(HttpMethod.Put, $"{_mainEndpoint}/{_roleIdToUpdate}", accessToken, jsonContent);

        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateRole_NonExistentRole_ReturnsNotFound()
    {
        var roleId = Guid.NewGuid();
        var payload = JsonContent.Create(new { displayName = "Updated Role Display" });

        var user = _fixture.GetSuperAdmin();
        Assert.NotNull(user);

        var (accessToken, refreshToken) = await TestUtils.Login(_client, user.Email!, _fixture.TestPassword);
        var request = TestUtils.GetRequestWithAuth(HttpMethod.Put, $"{_mainEndpoint}/{roleId}", accessToken, payload);

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion


    #region Delete Role
    [Theory]
    [InlineData(null, true)]
    [InlineData(null, false)]
    [InlineData(null, false)]
    public async Task DeleteRole_ValidInputs_ReturnsSuccess(Guid? id, bool s)
    {
        var user = _fixture.GetSuperAdmin();
        Assert.NotNull(user);

        var (accessToken, _) = await TestUtils.Login(_client, user.Email!, _fixture.TestPassword);
        var request = TestUtils.GetRequestWithAuth(HttpMethod.Delete, $"{_mainEndpoint}/{_roleIdToUpdate}", accessToken, null);

        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }


    #endregion


    #region Add Permission

    #endregion


    #region Remove Permission

    #endregion

    #region Auth Tests

    public static IEnumerable<object[]> AuthUseCases()
    {
        var guidId = Guid.NewGuid().ToString();
        var mainEndPointWithGuidId = _mainEndpoint + "/" + guidId;
        return new List<object[]>
        {
            // success are set to bad request because we will have invalid payload result
            // get endpoint
            new object[] { DefaultUserRoles.SuperAdmin, HttpMethod.Get, _mainEndpoint, HttpStatusCode.OK },
            new object[] { DefaultUserRoles.Admin, HttpMethod.Get, _mainEndpoint, HttpStatusCode.Forbidden },
            new object[] { DefaultUserRoles.User, HttpMethod.Get, _mainEndpoint, HttpStatusCode.Forbidden },
            new object[] { DefaultUserRoles.None, HttpMethod.Get, _mainEndpoint, HttpStatusCode.Unauthorized },

            // create endpoint
            new object[] { DefaultUserRoles.SuperAdmin, HttpMethod.Post, _mainEndpoint, HttpStatusCode.BadRequest },
            new object[] { DefaultUserRoles.Admin, HttpMethod.Post, _mainEndpoint, HttpStatusCode.Forbidden },
            new object[] { DefaultUserRoles.User, HttpMethod.Post, _mainEndpoint, HttpStatusCode.Forbidden },
            new object[] { DefaultUserRoles.None, HttpMethod.Post, _mainEndpoint, HttpStatusCode.Unauthorized },

            // update endpoint
            new object[] { DefaultUserRoles.SuperAdmin, HttpMethod.Put, mainEndPointWithGuidId, HttpStatusCode.BadRequest },
            new object[] { DefaultUserRoles.Admin, HttpMethod.Put, mainEndPointWithGuidId, HttpStatusCode.Forbidden },
            new object[] { DefaultUserRoles.User, HttpMethod.Put, mainEndPointWithGuidId, HttpStatusCode.Forbidden },
            new object[] { DefaultUserRoles.None, HttpMethod.Put, mainEndPointWithGuidId, HttpStatusCode.Unauthorized },

            // delete endpoint
            new object[] { DefaultUserRoles.SuperAdmin, HttpMethod.Delete, mainEndPointWithGuidId, HttpStatusCode.NotFound },
            new object[] { DefaultUserRoles.Admin, HttpMethod.Delete, mainEndPointWithGuidId, HttpStatusCode.Forbidden },
            new object[] { DefaultUserRoles.User, HttpMethod.Delete, mainEndPointWithGuidId, HttpStatusCode.Forbidden },
            new object[] { DefaultUserRoles.None, HttpMethod.Delete, mainEndPointWithGuidId, HttpStatusCode.Unauthorized },
        };
    }

    [Theory]
    [MemberData(nameof(AuthUseCases))]
    public async Task AuthTests(DefaultUserRoles role, HttpMethod method, string endpoint, HttpStatusCode expectedStatusCode)
    {
        var user = role switch
        {
            DefaultUserRoles.User => _fixture.GetUser(),
            DefaultUserRoles.Admin => _fixture.GetAdmin(),
            DefaultUserRoles.SuperAdmin => _fixture.GetSuperAdmin(),
            DefaultUserRoles.None => null,
            _ => null
        };

        _out.WriteLine(endpoint);

        (string, string)? userData = user != null ? (user.Email!, _fixture.TestPassword) : null;

        await TestUtils.TestAuth(_client, userData, method, endpoint, expectedStatusCode, null);
    }

    #endregion
}