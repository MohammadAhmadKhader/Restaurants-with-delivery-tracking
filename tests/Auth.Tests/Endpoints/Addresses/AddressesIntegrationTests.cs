using System.Net;
using System.Net.Http.Json;
using Auth.Contracts.Dtos.Address;
using Auth.Extensions.FluentValidationValidators;
using Auth.Models;
using Auth.Tests.Collections;
using Auth.Utils;
using Shared.Common;
using Shared.Utils;
using Xunit.Abstractions;

namespace Auth.Tests.Endpoints.Addresses;

[Collection("IntegrationTests")]
public class AddressesIntegrationTests(IntegrationTestsFixture fixture, ITestOutputHelper output)
{
    private readonly IntegrationTestsFixture _fixture = fixture;
    private readonly ITestOutputHelper _out = output;
    private readonly Guid _addressIdToDelete = fixture.Loader.Addresses.First().Id;
    private static readonly string _endpoint = "api/users/addresses";
    private readonly HttpClient _client = fixture.CreateClientWithTestOutput(output);

    #region Get User Addresses (Pagination)

    [Theory]
    [ClassData(typeof(PaginationTestData))]
    public async Task GetUserAddress_PaginationTests(int? page, int? size, int expectedPage, int expectedSize)
    {
        TestUtils.LogPayload(_out, [new { page = page?.ToString() ?? "null", size = size?.ToString() ?? "null", expectedPage, expectedSize }]);
        var user = _fixture.GetUser();
        Assert.NotNull(user);

        await PaginationTestUtils.TestPagination(_client, (user.Email, _fixture.TestPassword)!, _endpoint, page, size, expectedPage, expectedSize);
    }

    #endregion

    #region Create Address

    public static IEnumerable<object[]> CreateAddressInvalidInputs =>
    [
        // null validations
        [ CreateAddressDictionary("city", null), "city", ValidationMessagesBuilder.Required(nameof(AddressCreateDto.City)) ],
        [ CreateAddressDictionary("country", null),"country", ValidationMessagesBuilder.Required(nameof(AddressCreateDto.Country)) ],
        [ CreateAddressDictionary("addressLine", null), "addressLine", ValidationMessagesBuilder.Required(nameof(AddressCreateDto.AddressLine)) ],
        [ CreateAddressDictionary("postalCode", null), "postalCode", ValidationMessagesBuilder.Required(nameof(AddressCreateDto.PostalCode)) ],
        [ CreateAddressDictionary("latitude", null),"latitude", ValidationMessagesBuilder.Required(nameof(AddressCreateDto.Latitude)) ],
        [ CreateAddressDictionary("longitude", null), "longitude", ValidationMessagesBuilder.Required(nameof(AddressCreateDto.Longitude)) ],

        // validations against white spaces
        [ CreateAddressDictionary("city", new string(' ', Constants.MinCityLength)), "city",
        ValidationMessagesBuilder.Required(nameof(AddressCreateDto.City)) ],
        [ CreateAddressDictionary("country", new string(' ', Constants.MinCountryLength)), "country",
        ValidationMessagesBuilder.Required(nameof(AddressCreateDto.Country)) ],
        [ CreateAddressDictionary("state", new string(' ', Constants.MinStateLength - 1)),"state", LengthBetweenFormatter(nameof(AddressCreateDto.State)) ],
        [ CreateAddressDictionary("addressLine", new string(' ', Constants.MinAddressLineLength)), "addressLine",
        ValidationMessagesBuilder.Required(nameof(AddressCreateDto.AddressLine)) ],
        [ CreateAddressDictionary("postalCode", new string(' ', Constants.MinPostalCodeLength)), "postalCode",
        ValidationMessagesBuilder.Required(nameof(AddressCreateDto.PostalCode)) ],

        // length validations with invalid // * max length
        [ CreateAddressDictionary("city", new string('a', Constants.MaxCityLength + 1)), "city", LengthBetweenFormatter(nameof(AddressCreateDto.City)) ],
        [ CreateAddressDictionary("country", new string('a', Constants.MaxCountryLength + 1)),"country", LengthBetweenFormatter(nameof(AddressCreateDto.Country)) ],
        [ CreateAddressDictionary("state", new string('a', Constants.MaxStateLength + 1)),"state", LengthBetweenFormatter(nameof(AddressCreateDto.State)) ],
        [ CreateAddressDictionary("addressLine", new string('a', Constants.MaxAddressLineLength + 1)), "addressLine", LengthBetweenFormatter(nameof(AddressCreateDto.AddressLine)) ],
        [ CreateAddressDictionary("postalCode", new string('a', Constants.MaxPostalCodeLength + 1)), "postalCode", LengthBetweenFormatter(nameof(AddressCreateDto.PostalCode)) ],
        // decimal
        [ CreateAddressDictionary("longitude", Constants.MaxLongitude + 0.1), "longitude",
        ValidationMessagesBuilder.InclusiveBetween(nameof(AddressCreateDto.Longitude), Constants.MinLongitude, Constants.MaxLongitude)  ],
        [ CreateAddressDictionary("latitude", Constants.MaxLatitude + 0.1), "latitude",
        ValidationMessagesBuilder.InclusiveBetween(nameof(AddressCreateDto.Latitude), Constants.MinLatitude, Constants.MaxLatitude) ],

        // length validations with invalid // * min length
        [ CreateAddressDictionary("city", new string('a', Constants.MinCityLength - 1)), "city", LengthBetweenFormatter(nameof(AddressCreateDto.City)) ],
        [ CreateAddressDictionary("country", new string('a', Constants.MinCountryLength - 1)),"country", LengthBetweenFormatter(nameof(AddressCreateDto.Country)) ],
        [ CreateAddressDictionary("state", new string('a', Constants.MinStateLength - 1)),"state", LengthBetweenFormatter(nameof(AddressCreateDto.State)) ],
        [ CreateAddressDictionary("addressLine", new string('a', Constants.MinAddressLineLength - 1)), "addressLine", LengthBetweenFormatter(nameof(AddressCreateDto.AddressLine)) ],
        [ CreateAddressDictionary("postalCode", new string('a', Constants.MinPostalCodeLength - 1)), "postalCode", LengthBetweenFormatter(nameof(AddressCreateDto.PostalCode)) ],
        // decimal
        [ CreateAddressDictionary("longitude", Constants.MinLongitude - 0.1), "longitude",
        ValidationMessagesBuilder.InclusiveBetween(nameof(AddressCreateDto.Longitude), Constants.MinLongitude, Constants.MaxLongitude) ],
        [ CreateAddressDictionary("latitude", Constants.MinLatitude - 0.1), "latitude",
        ValidationMessagesBuilder.InclusiveBetween(nameof(AddressCreateDto.Latitude), Constants.MinLatitude, Constants.MaxLatitude) ],
    ];

    [Theory]
    [MemberData(nameof(CreateAddressInvalidInputs))]
    public async Task CreateAddress_InvalidInputs_ReturnsValidationError(Dictionary<string, object> payloadDict, string field, string expectedMessage)
    {
        TestUtils.LogPayload(_out, [new { payloadDict, field, expectedMessage }]);
        var jsonPayload = JsonContent.Create(payloadDict);

        var user = _fixture.GetSuperAdmin();
        Assert.NotNull(user);

        var response = await TestUtils.SendWithAuthAsync(_client, HttpMethod.Post, user.Email!,
         _fixture.TestPassword, _endpoint, jsonPayload);

        await TestUtils.AssertValidationError(response, field, expectedMessage);
    }

    [Fact]
    public async Task CreateAddress_ValidData_ReturnsSuccess()
    {
        var payload = JsonContent.Create(CreateAddressDictionary());

        var user = _fixture.GetSuperAdmin();
        Assert.NotNull(user);

        var response = await TestUtils.SendWithAuthAsync(_client, HttpMethod.Post, user.Email!,
         _fixture.TestPassword, _endpoint, payload);

        Assert.True(response.IsSuccessStatusCode, $"Expected success but got {response.StatusCode}");
    }

    #endregion

    #region Update Address

    public static IEnumerable<object[]> UpdateAddressInvalidInputs =>
    [
        // validations against white spaces
        [ CreateAddressDictionary("city", new string(' ', Constants.MinCityLength - 1)), "city", LengthBetweenFormatter(nameof(AddressUpdateDto.City)) ],
        [ CreateAddressDictionary("country", new string(' ', Constants.MinCountryLength - 1)),"country", LengthBetweenFormatter(nameof(AddressUpdateDto.Country)) ],
        [ CreateAddressDictionary("state", new string(' ', Constants.MinStateLength - 1)),"state", LengthBetweenFormatter(nameof(AddressUpdateDto.State)) ],
        [ CreateAddressDictionary("addressLine", new string(' ', Constants.MinAddressLineLength - 1)), "addressLine", LengthBetweenFormatter(nameof(AddressUpdateDto.AddressLine)) ],
        [ CreateAddressDictionary("postalCode", new string(' ', Constants.MinPostalCodeLength - 1)), "postalCode", LengthBetweenFormatter(nameof(AddressUpdateDto.PostalCode)) ],

        // length validations with invalid // * max length
        [ CreateAddressDictionary("city", new string('a', Constants.MaxCityLength + 1)), "city", LengthBetweenFormatter(nameof(AddressUpdateDto.City)) ],
        [ CreateAddressDictionary("country", new string('a', Constants.MaxCountryLength + 1)),"country", LengthBetweenFormatter(nameof(AddressUpdateDto.Country)) ],
        [ CreateAddressDictionary("state", new string('a', Constants.MaxStateLength + 1)),"state", LengthBetweenFormatter(nameof(AddressUpdateDto.State)) ],
        [ CreateAddressDictionary("addressLine", new string('a', Constants.MaxAddressLineLength + 1)), "addressLine", LengthBetweenFormatter(nameof(AddressUpdateDto.AddressLine)) ],
        [ CreateAddressDictionary("postalCode", new string('a', Constants.MaxPostalCodeLength + 1)), "postalCode", LengthBetweenFormatter(nameof(AddressUpdateDto.PostalCode)) ],

        // length validations with invalid // * min length
        [ CreateAddressDictionary("city", new string('a', Constants.MinCityLength - 1)), "city", LengthBetweenFormatter(nameof(AddressUpdateDto.City)) ],
        [ CreateAddressDictionary("country", new string('a', Constants.MinCountryLength - 1)),"country", LengthBetweenFormatter(nameof(AddressUpdateDto.Country)) ],
        [ CreateAddressDictionary("state", new string('a', Constants.MinStateLength - 1)),"state", LengthBetweenFormatter(nameof(AddressUpdateDto.State)) ],
        [ CreateAddressDictionary("addressLine", new string('a', Constants.MinAddressLineLength - 1)), "addressLine", LengthBetweenFormatter(nameof(AddressUpdateDto.AddressLine)) ],
        [ CreateAddressDictionary("postalCode", new string('a', Constants.MinPostalCodeLength - 1)), "postalCode", LengthBetweenFormatter(nameof(AddressUpdateDto.PostalCode)) ],

        // at least one required
        [ new Dictionary<string, object> {}, "address", AddressUpdateDtoValidator.AtLeastOneRequiredErrorMessage],
    ];

    [Theory]
    [MemberData(nameof(UpdateAddressInvalidInputs))]
    public async Task UpdateAddress_InvalidInputs_ReturnsValidationError(Dictionary<string, object> payloadDict, string field, string expectedMessage)
    {
        TestUtils.LogPayload(_out, [ new { payloadDict, field, expectedMessage }]);
        var addressId = Guid.NewGuid();
        var jsonPayload = JsonContent.Create(payloadDict);

        var user = _fixture.GetSuperAdmin();
        Assert.NotNull(user);

        var url = $"{_endpoint}/{addressId}";
        var response = await TestUtils.SendWithAuthAsync(_client, HttpMethod.Put, user.Email!,
         _fixture.TestPassword, url, jsonPayload);

        await TestUtils.AssertValidationError(response, field, expectedMessage);
    }

    [Fact]
    public async Task UpdateAddress_ValidData_ReturnsSuccess()
    {
        var addressId = Guid.NewGuid();
        var payload = JsonContent.Create(CreateAddressDictionary());

        var user = _fixture.GetSuperAdmin();
        Assert.NotNull(user);

        var url = $"{_endpoint}/{addressId}";
        var response = await TestUtils.SendWithAuthAsync(_client, HttpMethod.Put, user.Email!,
         _fixture.TestPassword, url, payload);

        Assert.True(response.StatusCode == HttpStatusCode.NotFound, $"Expected success but got {response.StatusCode}");
    }

    #endregion

    #region Delete Address

    [Fact]
    public async Task DeleteAddress_NotFoundAddress_ReturnsNotFoundError()
    {
        var addressId = Guid.NewGuid();
        var user = _fixture.GetUser();
        var url = $"{_endpoint}/{addressId}";
        var response = await TestUtils.SendWithAuthAsync(_client, HttpMethod.Delete, user.Email!, _fixture.TestPassword, url);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAddress_ValidDeletableAddress_ReturnsNoContent()
    {
        var user = _fixture.GetUser();
        Assert.NotNull(user);

        var url = $"{_endpoint}/{_addressIdToDelete}";
        var response = await TestUtils.SendWithAuthAsync(_client, HttpMethod.Delete, user.Email!, _fixture.TestPassword, url);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    #endregion


    #region Invalid Route Params

    public static IEnumerable<object[]> InvalidRouteParams()
    {
        var invalidId = "invalid-id";
        var endpointWithInvalidId = _endpoint + "/" + invalidId;

        return [
            [endpointWithInvalidId, HttpMethod.Put],
            [endpointWithInvalidId, HttpMethod.Delete]
        ];
    }

    [Theory]
    [MemberData(nameof(InvalidRouteParams))]
    public async Task InvalidRouteParams_ReturnsBadRequest(string endpoint, HttpMethod method)
    {
        TestUtils.LogPayload(_out, [new { endpoint, method }]);
        var user = _fixture.GetSuperAdmin();
        var response = await TestUtils.SendWithAuthAsync(_client, method, user.Email!, _fixture.TestPassword, endpoint);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region Auth Tests

    public static IEnumerable<object[]> AuthUseCases()
    {
        var guidId = Guid.NewGuid().ToString();
        var endpointWithGuidId = _endpoint + "/" + guidId;

        return
        [
            // get endpoint
            [DefaultUserRoles.User, HttpMethod.Get, _endpoint, HttpStatusCode.OK],
            [DefaultUserRoles.None, HttpMethod.Get, _endpoint, HttpStatusCode.Unauthorized],

            // create endpoint
            [DefaultUserRoles.User, HttpMethod.Post, _endpoint, HttpStatusCode.BadRequest],
            [DefaultUserRoles.None, HttpMethod.Post, _endpoint, HttpStatusCode.Unauthorized],

            // update endpoint
            [DefaultUserRoles.User, HttpMethod.Put, endpointWithGuidId, HttpStatusCode.BadRequest],
            [DefaultUserRoles.None, HttpMethod.Put, endpointWithGuidId, HttpStatusCode.Unauthorized],

            // delete endpoint
            [DefaultUserRoles.User, HttpMethod.Delete, endpointWithGuidId, HttpStatusCode.NotFound],
            [DefaultUserRoles.None, HttpMethod.Delete, endpointWithGuidId, HttpStatusCode.Unauthorized],
        ];
    }

    [Theory]
    [MemberData(nameof(AuthUseCases))]
    public async Task AuthTests(DefaultUserRoles role, HttpMethod method, string endpoint, HttpStatusCode expectedStatusCode)
    {
        TestUtils.LogPayload(_out, [ new { role = role.ToString(), method, endpoint, expectedStatusCode }]);
        var user = role switch
        {
            DefaultUserRoles.User => _fixture.GetUser(),
            DefaultUserRoles.None => null,
            _ => null
        };

        (string, string)? userData = user != null ? (user.Email!, _fixture.TestPassword) : null;
        await TestUtils.TestAuth(_client, userData, method, endpoint, expectedStatusCode, null);
    }

    #endregion

    private static string LengthBetweenFormatter(string property)
    {
        (int minLength, int maxLength) = property switch
        {
            nameof(AddressCreateDto.City) => (Constants.MinCityLength, Constants.MaxCityLength),
            nameof(AddressCreateDto.AddressLine) => (Constants.MinAddressLineLength, Constants.MaxAddressLineLength),
            nameof(AddressCreateDto.Country) => (Constants.MinCountryLength, Constants.MaxCountryLength),
            nameof(AddressCreateDto.State) => (Constants.MinStateLength, Constants.MaxStateLength),
            nameof(AddressCreateDto.PostalCode) => (Constants.MinPostalCodeLength, Constants.MaxPostalCodeLength),
            _ => throw new ArgumentException($"Invalid address property was provided property: {property}")
        };

        return ValidationMessagesBuilder.LengthBetween(property, minLength, maxLength);
    }

    public static Dictionary<string, object> CreateAddressDictionary(string? overrideKey = null, object? overrideValue = null)
    {
        var address = new Dictionary<string, object>
        {
            ["city"] = "City",
            ["addressLine"] = "AddressLine",
            ["state"] = "State",
            ["country"] = "Country",
            ["postalCode"] = "90210",
            ["latitude"] = 24.43,
            ["longitude"] = 24.43
        };

        if (overrideKey != null && address.ContainsKey(overrideKey))
        {
            address[overrideKey] = overrideValue!;
        }

        return address;
    }
}