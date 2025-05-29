using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Shared.Utils;

public class TestUtils
{
    /// <summary> This should be used with passwords only, for non passwords use new string('', length) </summary>
    public static string GenerateRandomString(int length)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable.Range(0, length).Select(_ => chars[random.Next(chars.Length)]).ToArray());
    }

    public static async Task AssertValidationError(HttpResponseMessage response, string expectedField, string expectedMessage)
    {
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        if (!doc.RootElement.TryGetProperty("errors", out var errorsElement))
        {
            Assert.Fail("Response does not contain 'errors' property");
        }

        var errors = errorsElement.EnumerateArray().ToList();

        Assert.Contains(errors, e =>
            e.GetProperty("field").GetString() == expectedField &&
            e.GetProperty("message").GetString() == expectedMessage
        );
    }

    public static async Task<(string accessToken, string refreshToken)> Login(HttpClient authClient, string email, string password)
    {
        var loginResp = await authClient.PostAsJsonAsync("/api/auth/login", new { email, password });

        Assert.Equal(HttpStatusCode.OK, loginResp.StatusCode);

        var respBody = await loginResp.Content.ReadFromJsonAsync<JsonElement>();
        var accessToken = respBody.GetProperty("access-token").GetString();
        var refreshToken = respBody.GetProperty("refresh-token").GetString();

        Assert.False(string.IsNullOrWhiteSpace(accessToken), "accessToken should not be null or whitespace");
        Assert.False(string.IsNullOrWhiteSpace(refreshToken), "refreshToken should not be null or whitespace");

        return (accessToken, refreshToken);
    }

    public static HttpRequestMessage GetRequestWithAuth(HttpMethod method, string endpoint, string accessToken, JsonContent payload = null)
    {
        var request = new HttpRequestMessage(method, endpoint)
        {
            Content = payload ?? JsonContent.Create(new { })
        };
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        return request;
    }
}