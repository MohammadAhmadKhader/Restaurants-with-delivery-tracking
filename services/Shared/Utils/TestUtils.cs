using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
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
            Assert.Fail("Response does not contain 'errors' field");
        }

        var errors = errorsElement.EnumerateArray().ToList();

        Assert.Contains(errors, e =>
            e.GetProperty("field").GetString() == expectedField &&
            e.GetProperty("message").GetString() == expectedMessage
        );
    }

    public static async Task<(string accessToken, string refreshToken)> Login(HttpClient authClient, string email, string password)
    {
        Assert.NotNull(email);
        Assert.NotNull(password);
        Assert.NotNull(authClient);
        var loginResp = await authClient.PostAsJsonAsync("/api/auth/login", new { email, password });

        Assert.Equal(HttpStatusCode.OK, loginResp.StatusCode);

        var respBody = await loginResp.Content.ReadFromJsonAsync<JsonElement>();
        var accessToken = respBody.GetProperty("accessToken").GetString();
        var refreshToken = respBody.GetProperty("refreshToken").GetString();

        Assert.False(string.IsNullOrWhiteSpace(accessToken), "accessToken should not be null or whitespace");
        Assert.False(string.IsNullOrWhiteSpace(refreshToken), "refreshToken should not be null or whitespace");

        return (accessToken, refreshToken);
    }

    public static HttpRequestMessage GetRequestWithAuth(HttpMethod method, string endpoint, string? accessToken, JsonContent? payload = null, bool assertToken = true)
    {
        if (assertToken)
        {
            Assert.NotNull(accessToken);
        }

        Assert.NotNull(endpoint);
        var request = new HttpRequestMessage(method, endpoint)
        {
            Content = payload ?? JsonContent.Create(new { })
        };

        if (accessToken != null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        return request;
    }

    public static string GetQueryString(Dictionary<string, string> dic)
    {
        return string.Join("&", dic.Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value)}"));
    }

    public static TData? Deserialize<TData>(JsonElement el, bool doAssertion = true)
    {
        var deserializedData = el.Deserialize<TData>(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        if (doAssertion)
        {
            Assert.NotNull(deserializedData);
        }

        return deserializedData;
    }

    public static PropertyInfo? GetProperty<TData>(string field, BindingFlags bindings = BindingFlags.Public | BindingFlags.Instance)
    {
        return typeof(TData).GetProperty(GeneralUtils.CamelToPascal(field), bindings);
    }

    public static async Task TestAuth(
        HttpClient authClient,
        (string email, string password)? userData,
        HttpMethod method,
        string endpoint,
        HttpStatusCode expectedStatusCode,
        JsonContent? payload = null
        )
    {
        string? accessToken = null;
        if (userData != null)
        {
            var (_accessToken, _) = await Login(authClient, userData?.email!, userData?.password!);
            accessToken = _accessToken;
        }

        var request = GetRequestWithAuth(method, endpoint, accessToken, payload, false);
        var response = await authClient.SendAsync(request);


        Assert.Equal(expectedStatusCode, response.StatusCode);
    }

    public static async Task<HttpResponseMessage> SendWithAuthAsync(HttpClient authClient, HttpMethod method, string email, string password, string endpoint, JsonContent? payload = null, Action<HttpRequestMessage>? modifier = null)
    {
        var (accessToken, _) = await Login(authClient, email, password);
        var request = GetRequestWithAuth(method, endpoint, accessToken, payload, true);

        modifier?.Invoke(request);

        var response = await authClient.SendAsync(request);
        return response;
    }
}