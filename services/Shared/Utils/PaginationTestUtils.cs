using System.Net;
using System.Net.Http.Json;
using Shared.Contracts.Dtos;
using Xunit;
namespace Shared.Utils;

public static class PaginationTestUtils
{
    public static async Task TestPagination(HttpClient client, (string Email, string Password) credentials,
    string endpoint, int? page, int? size, int expectedPage, int expectedSize)
    {
        var query = new List<string>();
        if (page.HasValue)
        {
            query.Add($"page={page.Value}");
        }

        if (size.HasValue)
        {
            query.Add($"size={size.Value}");
        }

        var url = endpoint + (query.Any() ? "?" + string.Join("&", query) : "");

        var (accessToken, _) = await TestUtils.Login(client, credentials.Email, credentials.Password);
        var request = TestUtils.GetRequestWithAuth(HttpMethod.Get, url, accessToken);

        var response = await client.SendAsync(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<object>>();
        Assert.Equal(expectedPage, result!.Page);
        Assert.Equal(expectedSize, result.Size);
        Assert.NotNull(result.Items);
    }
}

public class PaginationTestData : TheoryData<int?, int?, int, int>
{
    public PaginationTestData()
    {
        // page, size, expectedPage, expectedSize
        Add(null, null, PaginationUtils.DefaultPage, PaginationUtils.DefaultSize);
        Add(-1, 10, PaginationUtils.DefaultPage, PaginationUtils.DefaultSize);
        Add(1, -10, PaginationUtils.DefaultPage, PaginationUtils.DefaultSize);
        Add(1, PaginationUtils.MaxSizeAllowed + 10, PaginationUtils.DefaultPage, PaginationUtils.MaxSizeAllowed);
        Add(2, 5, 2, 5);
        Add(0, 0, PaginationUtils.DefaultPage, PaginationUtils.DefaultSize);
        Add(-5, -5, PaginationUtils.DefaultPage, PaginationUtils.DefaultSize);
        Add(PaginationUtils.DefaultPage, PaginationUtils.DefaultSize, PaginationUtils.DefaultPage, PaginationUtils.DefaultSize);
    }
}