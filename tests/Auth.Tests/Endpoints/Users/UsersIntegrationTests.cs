using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Auth.Dtos.User;
using Auth.Models;
using Auth.Tests.Collections;
using Auth.Utils;
using Shared.Common;
using Shared.Utils;
using Xunit.Abstractions;

namespace Auth.Tests.Endpoints.Users;

[Collection("IntegrationTests")]
public class UsersIntegrationTests(IntegrationTestsFixture fixture, ITestOutputHelper output)
{
    private readonly ITestOutputHelper _out = output;
    private readonly IntegrationTestsFixture _fixture = fixture;
    private static readonly string _endpoint = "api/users";
    private readonly HttpClient _client = fixture.CreateClientWithTestOutput(output);
    private const string collectionKey = "items";

    #region Get Users (Pagination)

    [Theory]
    [ClassData(typeof(PaginationTestData))]
    public async Task GetUsers_PaginationTests(int? page, int? size, int expectedPage, int expectedSize)
    {
        TestUtils.LogPayload(_out, [new { page = page?.ToString() ?? "null", size = size?.ToString() ?? "null", expectedPage, expectedSize }]);
        var user = _fixture.GetSuperAdmin();
        Assert.NotNull(user);

        await PaginationTestUtils.TestPagination(_client, (user.Email, _fixture.TestPassword)!, _endpoint, page, size, expectedPage, expectedSize);
    }

    #endregion


    #region Update Profile
    public static IEnumerable<object[]> InvalidUpdateUserProfile()
    {

        return [
            // null properties
            [
                new { firstName = (string?)null, lastName = new string('a', Constants.MinLastNameLength) },
                "firstName",
                ValidationMessagesBuilder.Required(nameof(UserUpdateProfile.FirstName))
            ],
            [
                new { firstName = new string('a', Constants.MinFirstNameLength), lastName = (string?)null },
                "lastName",
                ValidationMessagesBuilder.Required(nameof(UserUpdateProfile.LastName))
            ],

            // empty properties
            [
                new { firstName = "", lastName = new string('a', Constants.MinLastNameLength) },
                "firstName",
                FormatLengthBetween(nameof(UserUpdateProfile.FirstName))
            ],
            [
                new { firstName = new string('a', Constants.MinFirstNameLength), lastName = "" },
                "lastName",
                FormatLengthBetween(nameof(UserUpdateProfile.LastName))
            ],

            // whitespaces properties
            [
                new {
                    firstName = new string(' ', Constants.MinFirstNameLength),
                    lastName = new string('a', Constants.MinLastNameLength)
                },
                "firstName",
                FormatLengthBetween(nameof(UserUpdateProfile.FirstName))
            ],
            [
                new {
                    firstName = new string('a', Constants.MinFirstNameLength),
                    lastName = new string(' ', Constants.MinLastNameLength)
                },
                "lastName",
                FormatLengthBetween(nameof(UserUpdateProfile.LastName))
            ],

            // less than min length
            [   new {
                    firstName = new string('a', Constants.MinFirstNameLength - 1),
                    lastName = new string('a', Constants.MinLastNameLength)
                },
                "firstName",
                FormatLengthBetween(nameof(UserUpdateProfile.FirstName))
            ],
            [   new {
                    firstName = new string('a', Constants.MinFirstNameLength),
                    lastName = new string('a', Constants.MinLastNameLength - 1)
                },
                "lastName",
                FormatLengthBetween(nameof(UserUpdateProfile.LastName))
            ],

            // more than max length
            [
                new {
                    firstName = new string('a', Constants.MaxFirstNameLength + 1),
                    lastName = new string('a', Constants.MaxLastNameLength)
                },
                "firstName",
                FormatLengthBetween(nameof(UserUpdateProfile.FirstName))
            ],
            [
                new {
                    firstName = new string('a', Constants.MaxFirstNameLength),
                    lastName = new string('a', Constants.MaxLastNameLength + 1)
                },
                "lastName",
                FormatLengthBetween(nameof(UserUpdateProfile.LastName))
            ],

            // at least one is required
            [
                new { firstName = (string?) null, lastName = (string?) null }, "user",
                ValidationMessagesBuilder.AtLeastOneRequired(nameof(UserUpdateProfile.FirstName), nameof(UserUpdateProfile.LastName))
            ]
        ];
    }

    [Theory]
    [MemberData(nameof(InvalidUpdateUserProfile))]
    public async Task UpdateProfile_InvalidInputs_ReturnsValidationError(object payload, string expectedField, string expectedErrorMessage)
    {
        TestUtils.LogPayload(_out, [new { payload, expectedErrorMessage }]);
        var user = _fixture.GetUser();
        Assert.NotNull(user);

        var jsonPayload = JsonContent.Create(payload);
        var url = FormatUpdateProfileEndpoint();

        var response = await TestUtils.SendWithAuthAsync(_client, HttpMethod.Put, user!.Email!, _fixture.TestPassword, url, jsonPayload);
        await TestUtils.AssertValidationError(response, expectedField, expectedErrorMessage);
    }

    [Fact]
    public async Task UpdateProfile_ValidInput_ReturnsNoContent()
    {
        var firstName = new string('a', Constants.MinFirstNameLength);
        var lastName = new string('a', Constants.MinLastNameLength);
        var payload = JsonContent.Create(new
        {
            firstName,
            lastName,
        });

        var user = _fixture.GetUser();
        Assert.NotNull(user);

        var url = FormatUpdateProfileEndpoint();

        var response = await TestUtils.SendWithAuthAsync(_client, HttpMethod.Put, user!.Email!, _fixture.TestPassword, url, payload);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var updatedUser = await _fixture.GetUserFromDbByEmail(user.Email!);
        Assert.NotNull(updatedUser);
        Assert.Equal(firstName, updatedUser.FirstName);
        Assert.Equal(lastName, updatedUser.LastName);
    }

    #endregion

    #region Get User

    [Theory]
    [InlineData("SuperAdmin")]
    [InlineData("Admin")]
    public async Task GetUserById_AuthorizedRoles_ReturnsUserSuccessfully(string role)
    {
        TestUtils.LogPayload(_out, [new { role }]);
        var user = role switch
        {
            "SuperAdmin" => _fixture.GetSuperAdmin(),
            "Admin" => _fixture.GetAdmin(),
            _ => throw new ArgumentException("Unsupported role")
        };
        Assert.NotNull(user);
        var userId = _fixture.GetUser().Id;

        var url = FormatGetUserByIdEndpoint(userId);
        var response = await TestUtils.SendWithAuthAsync(_client, HttpMethod.Get, user!.Email!, _fixture.TestPassword, url);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);

        if (!doc.RootElement.TryGetProperty("user", out var usersJsonElement))
        {
            Assert.Fail("user were not returned");
        }

        var users = TestUtils.Deserialize<UserWithRolesAndPermissionsDto>(usersJsonElement);
        Assert.NotNull(users);
    }

    [Fact]
    public async Task GetUserById_NotExistentUser_ReturnsNotFound()
    {
        var user = _fixture.GetAdmin();
        Assert.NotNull(user);

        var url = FormatGetUserByIdEndpoint(Guid.NewGuid());
        var response = await TestUtils.SendWithAuthAsync(_client, HttpMethod.Get, user!.Email!, _fixture.TestPassword, url);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region Self Delete
    // * super admin and admin roles use cases are tested already in Auth Tests region

    [Fact]
    public async Task SelfDeleteUser_ValidRequest_ReturnsNoContent()
    {
        var randomUser = _fixture.GetRandomUser();

        var url = FormatSelfDeleteEndpoint();
        var response = await TestUtils.SendWithAuthAsync(_client, HttpMethod.Patch, randomUser!.Email!, _fixture.TestPassword, url);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var deletedUser = await _fixture.GetModelByPk<User>(randomUser.Id, ignoreFilters: true);
        AssertUserDelete(deletedUser);
    }

    #endregion

    #region Delete User

    [Theory]
    [InlineData("SuperAdmin")]
    [InlineData("Admin")]
    public async Task DeleteUserById_UnAllowedToDeleteRoles_ReturnsForbidden(string role)
    {
        TestUtils.LogPayload(_out, [new { role }]);
        var user = role switch
        {
            "SuperAdmin" => _fixture.GetSuperAdminToTryDelete(),
            "Admin" => _fixture.GetAdminToTryDelete(),
            _ => throw new ArgumentException("Unsupported role")
        };
        Assert.NotNull(user);

        var url = FormatDeleteUserByIdEndpoint(user.Id);
        var response = await TestUtils.SendWithAuthAsync(_client, HttpMethod.Patch, user!.Email!, _fixture.TestPassword, url);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeleteUserById_NotFoundUser_ReturnsNotFound()
    {
        var user = _fixture.GetSuperAdmin();
        Assert.NotNull(user);

        var url = FormatDeleteUserByIdEndpoint(Guid.NewGuid());
        var response = await TestUtils.SendWithAuthAsync(_client, HttpMethod.Patch, user!.Email!, _fixture.TestPassword, url);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteUserById_ValidRequest_ReturnsNoContent()
    {
        var superAdminUser = _fixture.GetSuperAdmin();
        var randomUser = _fixture.GetRandomUser();
        Assert.NotNull(superAdminUser);

        var url = FormatDeleteUserByIdEndpoint(randomUser.Id);
        var response = await TestUtils.SendWithAuthAsync(_client, HttpMethod.Patch, superAdminUser!.Email!, _fixture.TestPassword, url);

        _out.WriteLine(randomUser.Email);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var deletedUser = await _fixture.GetModelByPk<User>(randomUser.Id, ignoreFilters: true);
        AssertUserDelete(deletedUser);
    }

    #endregion


    #region Auth Tests

    public static IEnumerable<object[]> AuthUseCases()
    {
        var guidId = Guid.NewGuid();
        var findUserByIdEndpoint = FormatGetUserByIdEndpoint(guidId);
        var updateUserProfileEndpoint = FormatUpdateProfileEndpoint();
        var selfDeleteEndpoint = FormatSelfDeleteEndpoint();
        var deleteUserWithRandomIdEndpoint = FormatDeleteUserByIdEndpoint(guidId);

        return
        [
            // success are set to bad request because we will have invalid payload result
            // get users endpoint
            [DefaultUserRoles.SuperAdmin, HttpMethod.Get, _endpoint, HttpStatusCode.OK],
            [DefaultUserRoles.Admin, HttpMethod.Get, _endpoint, HttpStatusCode.OK],
            [DefaultUserRoles.User, HttpMethod.Get, _endpoint, HttpStatusCode.Forbidden],
            [DefaultUserRoles.None, HttpMethod.Get, _endpoint, HttpStatusCode.Unauthorized],

            // get user by id endpoint
            [DefaultUserRoles.SuperAdmin, HttpMethod.Get, findUserByIdEndpoint, HttpStatusCode.NotFound],
            [DefaultUserRoles.Admin, HttpMethod.Get, findUserByIdEndpoint, HttpStatusCode.NotFound],
            [DefaultUserRoles.User, HttpMethod.Get, findUserByIdEndpoint, HttpStatusCode.Forbidden],
            [DefaultUserRoles.None, HttpMethod.Get, findUserByIdEndpoint, HttpStatusCode.Unauthorized],

            // update profile
            [DefaultUserRoles.SuperAdmin, HttpMethod.Put, updateUserProfileEndpoint, HttpStatusCode.BadRequest],
            [DefaultUserRoles.Admin, HttpMethod.Put, updateUserProfileEndpoint, HttpStatusCode.BadRequest],
            [DefaultUserRoles.User, HttpMethod.Put, updateUserProfileEndpoint, HttpStatusCode.BadRequest],
            [DefaultUserRoles.None, HttpMethod.Put, updateUserProfileEndpoint, HttpStatusCode.Unauthorized],

            // delete user by id endpoint
            [DefaultUserRoles.SuperAdmin, HttpMethod.Patch, deleteUserWithRandomIdEndpoint, HttpStatusCode.NotFound],
            [DefaultUserRoles.Admin, HttpMethod.Patch, deleteUserWithRandomIdEndpoint, HttpStatusCode.NotFound],
            [DefaultUserRoles.User, HttpMethod.Patch, deleteUserWithRandomIdEndpoint, HttpStatusCode.Forbidden],
            [DefaultUserRoles.None, HttpMethod.Patch, deleteUserWithRandomIdEndpoint, HttpStatusCode.Unauthorized],

            // self-delete endpoint
            [DefaultUserRoles.SuperAdmin, HttpMethod.Patch, selfDeleteEndpoint, HttpStatusCode.Forbidden],
            [DefaultUserRoles.Admin, HttpMethod.Patch, selfDeleteEndpoint, HttpStatusCode.Forbidden],
            [DefaultUserRoles.User, HttpMethod.Patch, selfDeleteEndpoint, HttpStatusCode.NoContent],
            [DefaultUserRoles.None, HttpMethod.Patch, selfDeleteEndpoint, HttpStatusCode.Unauthorized],
        ];
    }

    [Theory]
    [MemberData(nameof(AuthUseCases))]
    public async Task AuthTests(DefaultUserRoles role, HttpMethod method, string endpoint, HttpStatusCode expectedStatusCode)
    {
        TestUtils.LogPayload(_out, [new { role = role.ToString(), method, endpoint, expectedStatusCode }]);
        var user = role switch
        {
            // ! do not user the normal users here just in case a user was delete it does not affect other tests
            DefaultUserRoles.User when method == HttpMethod.Patch => _fixture.GetRandomUser(),
            DefaultUserRoles.User => _fixture.GetUserToTryDelete(),
            DefaultUserRoles.Admin => _fixture.GetAdminToTryDelete(),
            DefaultUserRoles.SuperAdmin => _fixture.GetSuperAdminToTryDelete(),
            DefaultUserRoles.None => null,
            _ => null
        };

        (string, string)? userData = user != null ? (user.Email!, _fixture.TestPassword) : null;

        await TestUtils.TestAuth(_client, userData, method, endpoint, expectedStatusCode, null);
    }

    #endregion

    private static string FormatLengthBetween(string property)
    {
        var errMsg = property switch
        {
            nameof(UserUpdateProfile.FirstName) => ValidationMessagesBuilder.LengthBetween(nameof(UserUpdateProfile.FirstName),
            Constants.MinFirstNameLength, Constants.MaxFirstNameLength),
            nameof(UserUpdateProfile.LastName) => ValidationMessagesBuilder.LengthBetween(nameof(UserUpdateProfile.LastName),
            Constants.MinLastNameLength, Constants.MaxLastNameLength),
            _ => "",
        };

        return errMsg;
    }

    private static void AssertUserDelete(User? deletedUser)
    {
        Assert.True(deletedUser != null, "Deleted User was expected to be returned, received null");
        Assert.True(deletedUser!.IsDeleted);
        Assert.Null(deletedUser.FirstName);
        Assert.Null(deletedUser.LastName);
        Assert.Null(deletedUser.UserName);
        Assert.Null(deletedUser.NormalizedUserName);
        Assert.Null(deletedUser.Email);
        Assert.Null(deletedUser.NormalizedEmail);
        Assert.Null(deletedUser.PasswordHash);
        Assert.Null(deletedUser.PhoneNumber);
    }

    private static string FormatSelfDeleteEndpoint()
    {
        return $"{_endpoint}/self-delete";
    }

    private static string FormatDeleteUserByIdEndpoint(Guid userId)
    {
        return $"{_endpoint}/delete/{userId}";
    }

    private static string FormatGetUserByIdEndpoint(Guid userId)
    {
        return $"{_endpoint}/{userId}";
    }

    private static string FormatUpdateProfileEndpoint()
    {
        return $"{_endpoint}/profile";
    }
}