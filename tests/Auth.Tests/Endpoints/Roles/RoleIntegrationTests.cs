using System.Net;
using System.Net.Http.Json;
using Auth.Data;
using Auth.Extensions.FluentValidationValidators;
using Auth.Models;
using Auth.Repositories.IRepositories;
using Auth.Tests.Collections;
using Auth.Utils;
using Microsoft.Extensions.DependencyInjection;
using Shared.Common;
using Shared.Data.Patterns.UnitOfWork;
using Shared.Utils;
using Xunit.Abstractions;

namespace Auth.Tests.Endpoints.Roles;

[Collection("IntegrationTests")]
public class RoleIntegrationTests(IntegrationTestsFixture fixture, ITestOutputHelper output) : IAsyncLifetime
{
    private readonly IntegrationTestsFixture _fixture = fixture;
    private readonly ITestOutputHelper _out = output;
    private readonly HttpClient _client = fixture.CreateClientWithTestOutput(output);
    private static readonly string _mainEndpoint = "api/roles";
    private readonly Lock _initLock = new();
    private static bool _testDataInitialized = false;
    private static Guid _roleIdToUpdate;
    private static Guid _roleIdToDelete;
    private static Guid _roleIdToAddPermissions1;
    private static Guid _roleIdToAddPermissions2;
    private static Guid _roleIdToRemovePermission1;
    private static Guid _roleIdToRemovePermission2;
    private static int _permissionIdToBeRemoved1;
    private static int _permissionIdToBeRemoved2;
    private static int _permissionIdToBeRemoved3;

    public async Task InitializeAsync()
    {
        lock (_initLock)
        {
            if (_testDataInitialized)
            {
                return;
            }

            _testDataInitialized = true;
        }

        var roleToUpdate = new Role
        {
            Name = "NewRolex1",
            DisplayName = "NewRolex1"
        };

        var roleToDelete = new Role
        {
            Name = "NewRolex2",
            DisplayName = "NewRolex2"
        };

        var roleToAddPermissions1 = new Role
        {
            Name = "NewRolex3",
            DisplayName = "NewRolex3"
        };

        var roleToAddPermissions2 = new Role
        {
            Name = "NewRolex4",
            DisplayName = "NewRolex4"
        };

        var roleToRemovePermission1 = new Role
        {
            Name = "NewRolex5",
            DisplayName = "NewRolex5"
        };

        var roleToRemovePermission2 = new Role
        {
            Name = "NewRolex6",
            DisplayName = "NewRolex6"
        };

        var permissionToBeRemoved1 = new Permission
        {
            Name = "NewPermission1",
            DisplayName = "NewPermission1",
            IsDefaultUser = false,
            IsDefaultAdmin = true,
            IsDefaultSuperAdmin = true,
        };

        var permissionToBeRemoved2 = new Permission
        {
            Name = "NewPermission2",
            DisplayName = "NewPermission2",
            IsDefaultUser = false,
            IsDefaultAdmin = true,
            IsDefaultSuperAdmin = true,
        };

        // this will not be added to any role, will be used with removale of not added permission attempt
        var permissionToBeRemoved3 = new Permission
        {
            Name = "NewPermission3",
            DisplayName = "NewPermission3",
            IsDefaultUser = false,
            IsDefaultAdmin = true,
            IsDefaultSuperAdmin = true,
        };

        using var scope = _fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork<AppDbContext>>();
        var rolesRepository = scope.ServiceProvider.GetRequiredService<IRolesRepository>();
        var permissionsRepository = scope.ServiceProvider.GetRequiredService<IPermissionsRepository>();

        using var transaction = await unitOfWork.BeginTransactionAsync();
        await rolesRepository.CreateAsync(roleToUpdate);
        await rolesRepository.CreateAsync(roleToDelete);
        await rolesRepository.CreateAsync(roleToAddPermissions1);
        await rolesRepository.CreateAsync(roleToAddPermissions2);
        await rolesRepository.CreateAsync(roleToRemovePermission1);
        await rolesRepository.CreateAsync(roleToRemovePermission2);

        await permissionsRepository.CreateAsync(permissionToBeRemoved1);
        await permissionsRepository.CreateAsync(permissionToBeRemoved2);
        await permissionsRepository.CreateAsync(permissionToBeRemoved3);

        await unitOfWork.SaveChangesAsync();

        roleToRemovePermission1.Permissions.Add(permissionToBeRemoved1);
        roleToRemovePermission2.Permissions.Add(permissionToBeRemoved2);
        await unitOfWork.SaveChangesAsync();
        await transaction.CommitAsync();

        _roleIdToUpdate = roleToUpdate.Id;
        _roleIdToDelete = roleToDelete.Id;
        _roleIdToAddPermissions1 = roleToAddPermissions1.Id;
        _roleIdToAddPermissions2 = roleToAddPermissions2.Id;
        _roleIdToRemovePermission1 = roleToRemovePermission1.Id;
        _roleIdToRemovePermission2 = roleToRemovePermission2.Id;
        _permissionIdToBeRemoved1 = permissionToBeRemoved1.Id;
        _permissionIdToBeRemoved2 = permissionToBeRemoved2.Id;
        _permissionIdToBeRemoved3 = permissionToBeRemoved3.Id;
    }

    public Task DisposeAsync() => Task.CompletedTask;

    #region Get Roles (Pagination)

    [Theory]
    [ClassData(typeof(PaginationTestData))]
    public async Task GetRoles_PaginationTests(int? page, int? size, int expectedPage, int expectedSize)
    {
        TestUtils.LogPayload(_out, [new { page = page?.ToString() ?? "null", size = size?.ToString() ?? "null", expectedPage, expectedSize }]);
        var user = _fixture.GetSuperAdmin();
        Assert.NotNull(user);

        await PaginationTestUtils.TestPagination(_client, (user.Email, _fixture.TestPassword)!, _mainEndpoint, page, size, expectedPage, expectedSize);
    }

    #endregion

    #region Create Role

    public static IEnumerable<object[]> CreateRoleInvalidInputs =>
    [
        [ new Dictionary<string, object> { ["name"] = null!, ["displayName"] = "DisplayName" },
            "name", ValidationMessagesBuilder.Required(nameof(Role.Name)) ],
        [ new Dictionary<string, object> { ["name"] = "Name", ["displayName"] = null! },
            "displayName", ValidationMessagesBuilder.Required(nameof(Role.DisplayName)) ],
        [ new Dictionary<string, object> { ["name"] = new string('a', Constants.MinRoleNameLength - 1) },
            "name", LengthBetweenFormatter(nameof(Role.Name)) ],
        [ new Dictionary<string, object> { ["name"] = new string('a', Constants.MinRoleNameLength), ["displayName"] = new string('a', Constants.MinRoleNameLength - 1) },
            "displayName", LengthBetweenFormatter(nameof(Role.DisplayName)) ],
        [ new Dictionary<string, object> { ["name"] = new string('x', Constants.MaxRoleNameLength + 1) },
            "name", LengthBetweenFormatter(nameof(Role.Name)) ],
        [ new Dictionary<string, object> { ["name"] = new string('x', Constants.MaxRoleNameLength), ["displayName"] = new string('x', Constants.MaxRoleNameLength + 1) },
            "displayName", LengthBetweenFormatter(nameof(Role.DisplayName)) ]
    ];

    [Theory]
    [MemberData(nameof(CreateRoleInvalidInputs))]
    public async Task CreateRole_InvalidInputas_ReturnsValidationError(Dictionary<string, object> payloadDict, string field, string expectedMessage)
    {
        TestUtils.LogPayload(_out, [new { payloadDict, field, expectedMessage }]);
        var jsonPayload = JsonContent.Create(payloadDict);

        var user = _fixture.GetSuperAdmin();
        Assert.NotNull(user);

        var response = await TestUtils.SendWithAuthAsync(_client, HttpMethod.Post, user.Email!,
         _fixture.TestPassword, _mainEndpoint, jsonPayload);

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

        var response = await TestUtils.SendWithAuthAsync(_client, HttpMethod.Post, user.Email!,
         _fixture.TestPassword, _mainEndpoint, payload);

        Assert.True(response.IsSuccessStatusCode, $"Expected success but got {response.StatusCode}");
    }

    #endregion

    #region Update Role

    public static IEnumerable<object[]> UpdateRoleInvalidInputs =>
    [
        ["displayName", new { displayName = new string('a', Constants.MinRoleNameLength - 1) }, LengthBetweenFormatter(nameof(Role.DisplayName))],

        ["displayName", new { displayName =  new string('x', Constants.MaxRoleNameLength + 1) }, LengthBetweenFormatter(nameof(Role.DisplayName))],

        ["name", new { name = new string('a', Constants.MinRoleNameLength - 1) }, LengthBetweenFormatter(nameof(Role.Name))],

        ["name", new { name = new string('x', Constants.MaxRoleNameLength + 1)}, LengthBetweenFormatter(nameof(Role.Name))],

        ["role", new { displayName = "", name = "" }, RoleUpdateDtoValidator.AtLeastOneRequiredErrorMessage],

        ["role", new { displayName = "   ", name = "   " }, RoleUpdateDtoValidator.AtLeastOneRequiredErrorMessage],

        ["role", new { displayName = (string?)null, name = (string?)null }, RoleUpdateDtoValidator.AtLeastOneRequiredErrorMessage],
    ];

    [Theory]
    [MemberData(nameof(UpdateRoleInvalidInputs))]
    public async Task UpdateRole_InvalidInputs_ReturnsValidationError(string fieldName, object payload, string expectedError)
    {
        TestUtils.LogPayload(_out, [new { fieldName, payload, expectedError }]);
        var roleId = Guid.NewGuid();
        var user = _fixture.GetSuperAdmin();
        Assert.NotNull(user);

        var jsonPayload = JsonContent.Create(payload);

        var url = $"{_mainEndpoint}/{roleId}";
        var response = await TestUtils.SendWithAuthAsync(_client, HttpMethod.Put, user.Email!, _fixture.TestPassword, url, jsonPayload);

        await TestUtils.AssertValidationError(response, fieldName, expectedError);
    }

    [Theory]
    [InlineData(new string[] { "name" }, new string[] { "Updated Role Namex1" })]
    [InlineData(new string[] { "displayName" }, new string[] { "Updated Role Display Namex2" })]
    [InlineData(new string[] { "name", "displayName" }, new string[] { "Updated Role Namex3", "Updated Role Display Namex3" })]
    public async Task UpdateRole_ValidInputs_ReturnsSuccess(string[] fieldName, string[] fieldValue)
    {
        TestUtils.LogPayload(_out, [new { fieldName, fieldValue }]);
        var user = _fixture.GetSuperAdmin();
        Assert.NotNull(user);

        var payload = new Dictionary<string, string>();
        for (int i = 0; i < fieldName.Length; i++)
        {
            payload[fieldName[i]] = fieldValue[i];
        }
        var jsonPayload = JsonContent.Create(payload);

        TestUtils.LogPayload(_out, [new { payload }]);

        var url = $"{_mainEndpoint}/{_roleIdToUpdate}";
        var response = await TestUtils.SendWithAuthAsync(_client, HttpMethod.Put, user.Email!, _fixture.TestPassword, url, jsonPayload);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateRole_NonExistentRole_ReturnsNotFound()
    {
        var roleId = Guid.NewGuid();
        var payload = JsonContent.Create(new { displayName = "Updated Role Display" });

        var user = _fixture.GetSuperAdmin();
        Assert.NotNull(user);

        var url = $"{_mainEndpoint}/{roleId}";
        var response = await TestUtils.SendWithAuthAsync(_client, HttpMethod.Put, user.Email!, _fixture.TestPassword, url, payload);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion


    #region Delete Role

    [Theory]
    [InlineData("NotFound")]
    [InlineData("BadRequest")]
    public async Task DeleteRole_InvalidAttempts_ReturnsProperError(string scenario)
    {
        Guid roleId;
        HttpStatusCode expectedStatus;

        switch (scenario)
        {
            case "NotFound":
                roleId = Guid.NewGuid();
                expectedStatus = HttpStatusCode.NotFound;
                break;

            case "BadRequest":
                var superAdmin = _fixture.Loader.Roles.First(r => r.Name == RolePolicies.SuperAdmin);
                roleId = superAdmin.Id;
                expectedStatus = HttpStatusCode.BadRequest;
                break;

            default:
                throw new NotSupportedException(nameof(scenario));
        }
        TestUtils.LogPayload(_out, [new { roleId, expectedStatus }]);

        var user = _fixture.GetSuperAdmin();
        var url = $"{_mainEndpoint}/{roleId}";
        var response = await TestUtils.SendWithAuthAsync(_client, HttpMethod.Delete, user.Email!, _fixture.TestPassword, url);

        Assert.Equal(expectedStatus, response.StatusCode);
    }

    [Fact]
    public async Task DeleteRole_ValidDeletableRole_ReturnsNoContent()
    {
        var user = _fixture.GetSuperAdmin();
        Assert.NotNull(user);

        var url = $"{_mainEndpoint}/{_roleIdToDelete}";
        var response = await TestUtils.SendWithAuthAsync(_client, HttpMethod.Delete, user.Email!, _fixture.TestPassword, url);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    #endregion


    #region Add Permission

    [Fact]
    public async Task AddPermission_RoleNotFound_ReturnsNotFound()
    {
        var fakeRoleId = Guid.NewGuid();
        var user = _fixture.GetSuperAdmin();
        var url = FormatAddPermissionEndpoint(fakeRoleId);
        var body = new Dictionary<string, int[]> { { "ids", new[] { _fixture.Loader.NotSuperAdminOnlyPermissions.First().Id } } };

        var response = await TestUtils.SendWithAuthAsync(_client, HttpMethod.Post, user.Email!,
            _fixture.TestPassword, url, JsonContent.Create(body));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddPermission_SuperAdminOnlyPermission_ReturnsBadRequest()
    {
        var user = _fixture.GetSuperAdmin();
        var roleId = _fixture.Loader.AdminRole.Id;
        var url = FormatAddPermissionEndpoint(roleId);
        var body = new Dictionary<string, int[]> { { "ids", new[] { _fixture.Loader.SuperAdminOnlyPermission.Id } } };

        var response = await TestUtils.SendWithAuthAsync(_client, HttpMethod.Post, user.Email!,
            _fixture.TestPassword, url, JsonContent.Create(body));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AddPermission_OneofThePermissionsNotFound_ReturnsBadRequest()
    {
        var user = _fixture.GetSuperAdmin();
        var roleId = _fixture.Loader.AdminRole.Id;
        var url = FormatAddPermissionEndpoint(roleId);
        var fakePermissionId = 999999;
        var body = new Dictionary<string, int[]> { { "ids", new[] { fakePermissionId, _fixture.Loader.NotSuperAdminOnlyPermissions.First().Id } } };

        var response = await TestUtils.SendWithAuthAsync(_client, HttpMethod.Post, user.Email!,
            _fixture.TestPassword, url, JsonContent.Create(body));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddPermission_OnePermissionAndIsNotFound_ReturnsBadRequest()
    {
        var user = _fixture.GetSuperAdmin();
        var roleId = _fixture.Loader.AdminRole.Id;
        var url = FormatAddPermissionEndpoint(roleId);
        var fakePermissionId = 999999;
        var body = new Dictionary<string, int[]> { { "ids", new[] { fakePermissionId } } };

        var response = await TestUtils.SendWithAuthAsync(_client, HttpMethod.Post, user.Email!,
            _fixture.TestPassword, url, JsonContent.Create(body));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddPermission_PermissionsWithOneIsAlreadyAssigned_ReturnsConflict()
    {
        var user = _fixture.GetSuperAdmin();
        var roleId = _fixture.Loader.AdminRole.Id;
        var url = FormatAddPermissionEndpoint(roleId);

        var assignedPermission = _fixture.Loader.AdminRole.Permissions.First();
        var notAssignedPermission = _fixture.Loader.Permissions.First(x => x.Name == TestDataLoader.TestPermissionNameX1);
        var body = new Dictionary<string, int[]> { { "ids", new[] { assignedPermission.Id, notAssignedPermission.Id } } };

        var response = await TestUtils.SendWithAuthAsync(_client, HttpMethod.Post, user.Email!,
            _fixture.TestPassword, url, JsonContent.Create(body));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task AddPermission_OnePermissionThatIsAlreadyAssigned_ReturnsConflict()
    {
        var user = _fixture.GetSuperAdmin();
        var roleId = _fixture.Loader.AdminRole.Id;
        var url = FormatAddPermissionEndpoint(roleId);

        var assignedPermission = _fixture.Loader.AdminRole.Permissions.First();
        var body = new Dictionary<string, int[]> { { "ids", new[] { assignedPermission.Id } } };

        var response = await TestUtils.SendWithAuthAsync(_client, HttpMethod.Post, user.Email!,
            _fixture.TestPassword, url, JsonContent.Create(body));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task AddPermission_AddingOnePermissionCorrectly_ReturnsNoContent()
    {
        var user = _fixture.GetSuperAdmin();
        var roleId = _roleIdToAddPermissions1;
        var url = FormatAddPermissionEndpoint(roleId);

        var notAssignedPermission1 = _fixture.Loader.NotSuperAdminOnlyPermissions.First(x => x.Name == TestDataLoader.TestPermissionNameX1);
        var body = new Dictionary<string, int[]> { { "ids", new[] { notAssignedPermission1.Id } } };

        var response = await TestUtils.SendWithAuthAsync(_client, HttpMethod.Post, user.Email!,
            _fixture.TestPassword, url, JsonContent.Create(body));

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task AddPermission_AddingMultiplePermissionsCorrectly_ReturnsNoContent()
    {
        var user = _fixture.GetSuperAdmin();
        var roleId = _roleIdToAddPermissions2;
        var url = FormatAddPermissionEndpoint(roleId);

        var notAssignedPermission1 = _fixture.Loader.Permissions.First(x => x.Name == TestDataLoader.TestPermissionNameX1);
        var notAssignedPermission2 = _fixture.Loader.Permissions.First(x => x.Name == TestDataLoader.TestPermissionNameX2);
        var body = new Dictionary<string, int[]> { { "ids", new[] { notAssignedPermission1.Id, notAssignedPermission2.Id } } };

        var response = await TestUtils.SendWithAuthAsync(_client, HttpMethod.Post, user.Email!,
            _fixture.TestPassword, url, JsonContent.Create(body));

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    #endregion


    #region Remove Permission

    [Fact]
    public async Task RemovePermission_NotExistingRole_ReturnsNotFound()
    {
        var user = _fixture.GetSuperAdmin();
        var roleId = Guid.NewGuid();
        var url = FormatRemovePermissionEndpoint(roleId, _permissionIdToBeRemoved1);

        var response = await TestUtils.SendWithAuthAsync(_client, HttpMethod.Delete, user.Email!, _fixture.TestPassword, url);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RemovePermission_NotExistingPermission_ReturnsNotFound()
    {
        var user = _fixture.GetSuperAdmin();
        var roleId = _roleIdToRemovePermission1;
        var notExistingPermissionId = 999999;
        var url = FormatRemovePermissionEndpoint(roleId, notExistingPermissionId);

        var response = await TestUtils.SendWithAuthAsync(_client, HttpMethod.Delete, user.Email!, _fixture.TestPassword, url);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RemovePermission_RemovingPermissionFromSuperAdminRole_ReturnsBadRequest()
    {
        var user = _fixture.GetSuperAdmin();
        var superAdminRole = _fixture.Loader.SuperAdminRole;
        var roleId = superAdminRole.Id;
        var url = FormatRemovePermissionEndpoint(roleId, superAdminRole.Permissions.First().Id);

        var response = await TestUtils.SendWithAuthAsync(_client, HttpMethod.Delete, user.Email!, _fixture.TestPassword, url);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task RemovePermission_AttemptingToRemoveNotAddedPermission_ReturnsBadRequest()
    {
        var user = _fixture.GetSuperAdmin();
        var adminRole = _fixture.Loader.AdminRole;
        var roleId = adminRole.Id;
        var url = FormatRemovePermissionEndpoint(roleId, _permissionIdToBeRemoved3);

        var response = await TestUtils.SendWithAuthAsync(_client, HttpMethod.Delete, user.Email!, _fixture.TestPassword, url);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task RemovePermission_ValidRemovale_ReturnsNoContent()
    {
        var user = _fixture.GetSuperAdmin();
        var roleId = _roleIdToRemovePermission2;
        var notExistingPermissionId = _permissionIdToBeRemoved2;
        var url = FormatRemovePermissionEndpoint(roleId, notExistingPermissionId);

        var response = await TestUtils.SendWithAuthAsync(_client, HttpMethod.Delete, user.Email!, _fixture.TestPassword, url);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }


    #endregion

    
    #region Invalid Route Params

    public static IEnumerable<object[]> InvalidRouteParams() {
        var invalidId = "invalid-id";
        var roleEndPointWithInvalidGuid = _mainEndpoint + "/" + invalidId;
        var removePermFromRoleWithInvalidPermId = _mainEndpoint + "/" + Guid.NewGuid().ToString() + "/permissions/" + invalidId;
        var addPermToRoleWithInvalidRoleId = _mainEndpoint + "/" + invalidId + "/permissions";

        return [
            [roleEndPointWithInvalidGuid, HttpMethod.Put],
            [roleEndPointWithInvalidGuid, HttpMethod.Delete],
            [addPermToRoleWithInvalidRoleId, HttpMethod.Post],
            [removePermFromRoleWithInvalidPermId, HttpMethod.Delete]
        ];
    }

    [Theory]
    [MemberData(nameof(InvalidRouteParams))]
    public async Task InvalidRouteParams_ReturnsBadRequest(string endpoint, HttpMethod method)
    {
        TestUtils.LogPayload(_out, [new { method, endpoint }]);
        var user = _fixture.GetSuperAdmin();
        var response = await TestUtils.SendWithAuthAsync(_client, method, user.Email!, _fixture.TestPassword, endpoint);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion


    #region Auth Tests

    public static IEnumerable<object[]> AuthUseCases()
    {
        var guidId = Guid.NewGuid().ToString();
        var mainEndPointWithGuidId = _mainEndpoint + "/" + guidId;
        var addPermissionEndPoint = FormatAddPermissionEndpoint(Guid.NewGuid());
        var addPermissionEndPointWithPermissionId = FormatRemovePermissionEndpoint(Guid.NewGuid(), 9999999);
        return
        [
            // success are set to bad request because we will have invalid payload result
            // get endpoint
            [DefaultUserRoles.SuperAdmin, HttpMethod.Get, _mainEndpoint, HttpStatusCode.OK],
            [DefaultUserRoles.Admin, HttpMethod.Get, _mainEndpoint, HttpStatusCode.Forbidden],
            [DefaultUserRoles.User, HttpMethod.Get, _mainEndpoint, HttpStatusCode.Forbidden],
            [DefaultUserRoles.None, HttpMethod.Get, _mainEndpoint, HttpStatusCode.Unauthorized],

            // create endpoint
            [DefaultUserRoles.SuperAdmin, HttpMethod.Post, _mainEndpoint, HttpStatusCode.BadRequest],
            [DefaultUserRoles.Admin, HttpMethod.Post, _mainEndpoint, HttpStatusCode.Forbidden],
            [DefaultUserRoles.User, HttpMethod.Post, _mainEndpoint, HttpStatusCode.Forbidden],
            [DefaultUserRoles.None, HttpMethod.Post, _mainEndpoint, HttpStatusCode.Unauthorized],

            // update endpoint
            [DefaultUserRoles.SuperAdmin, HttpMethod.Put, mainEndPointWithGuidId, HttpStatusCode.BadRequest],
            [DefaultUserRoles.Admin, HttpMethod.Put, mainEndPointWithGuidId, HttpStatusCode.Forbidden],
            [DefaultUserRoles.User, HttpMethod.Put, mainEndPointWithGuidId, HttpStatusCode.Forbidden],
            [DefaultUserRoles.None, HttpMethod.Put, mainEndPointWithGuidId, HttpStatusCode.Unauthorized],

            // delete endpoint
            [DefaultUserRoles.SuperAdmin, HttpMethod.Delete, mainEndPointWithGuidId, HttpStatusCode.NotFound],
            [DefaultUserRoles.Admin, HttpMethod.Delete, mainEndPointWithGuidId, HttpStatusCode.Forbidden],
            [DefaultUserRoles.User, HttpMethod.Delete, mainEndPointWithGuidId, HttpStatusCode.Forbidden],
            [DefaultUserRoles.None, HttpMethod.Delete, mainEndPointWithGuidId, HttpStatusCode.Unauthorized],

            // create permission endpoint
            [DefaultUserRoles.SuperAdmin, HttpMethod.Post, addPermissionEndPoint, HttpStatusCode.BadRequest],
            [DefaultUserRoles.Admin, HttpMethod.Post, addPermissionEndPoint, HttpStatusCode.Forbidden],
            [DefaultUserRoles.User, HttpMethod.Post, addPermissionEndPoint, HttpStatusCode.Forbidden],
            [DefaultUserRoles.None, HttpMethod.Post, addPermissionEndPoint, HttpStatusCode.Unauthorized],

            // remove permission endpoint
            [DefaultUserRoles.SuperAdmin, HttpMethod.Delete, addPermissionEndPointWithPermissionId, HttpStatusCode.NotFound],
            [DefaultUserRoles.Admin, HttpMethod.Delete, addPermissionEndPointWithPermissionId, HttpStatusCode.Forbidden],
            [DefaultUserRoles.User, HttpMethod.Delete, addPermissionEndPointWithPermissionId, HttpStatusCode.Forbidden],
            [DefaultUserRoles.None, HttpMethod.Delete, addPermissionEndPointWithPermissionId, HttpStatusCode.Unauthorized],
        ];
    }

    [Theory]
    [MemberData(nameof(AuthUseCases))]
    public async Task AuthTests(DefaultUserRoles role, HttpMethod method, string endpoint, HttpStatusCode expectedStatusCode)
    {
        TestUtils.LogPayload(_out, [new { role = role.ToString(), method, endpoint, expectedStatusCode }]);
        var user = role switch
        {
            DefaultUserRoles.User => _fixture.GetUser(),
            DefaultUserRoles.Admin => _fixture.GetAdmin(),
            DefaultUserRoles.SuperAdmin => _fixture.GetSuperAdmin(),
            DefaultUserRoles.None => null,
            _ => null
        };

        (string, string)? userData = user != null ? (user.Email!, _fixture.TestPassword) : null;

        await TestUtils.TestAuth(_client, userData, method, endpoint, expectedStatusCode, null);
    }

    #endregion

    private static string FormatRemovePermissionEndpoint(Guid roleId, int permissionId)
    {
        Assert.False(Guid.Empty == roleId);
        return _mainEndpoint + "/" + roleId.ToString() + "/" + "permissions" + "/" + permissionId;
    }

    private static string FormatAddPermissionEndpoint(Guid roleId)
    {
        Assert.False(Guid.Empty == roleId);
        return _mainEndpoint + "/" + roleId.ToString() + "/" + "permissions";
    }

    private static string LengthBetweenFormatter(string fieldName) =>
        ValidationMessagesBuilder.LengthBetween(fieldName, Constants.MinRoleNameLength, Constants.MaxRoleNameLength);
}