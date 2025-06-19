using System.Text;
using System.Text.Json;
using Shared.Common;
using Shared.Contracts.Interfaces;

namespace Shared.Http;

public class HttpClientService : IHttpClientService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    public HttpClientService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = AppJsonSerializerOptions.Options;
    }
    public async Task<TResponse?> GetAsync<TResponse>(string endpoint, Dictionary<string, string>? headers = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

        AddHeaders(request, headers);

        var response = await _httpClient.SendAsync(request);
        return await HandleResponse<TResponse>(response);
    }

    public async Task<TResponse?> PostAsync<TResponse>(string endpoint, object? data = null, Dictionary<string, string>? headers = null)
    {
        return await PostAsync<object, TResponse>(endpoint, data!, headers);
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data, Dictionary<string, string>? headers = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
        if (data != null)
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        AddHeaders(request, headers);

        var response = await _httpClient.SendAsync(request);
        return await HandleResponse<TResponse>(response);
    }

    
    public async Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest data, Dictionary<string, string>? headers = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, endpoint);
        if (data != null)
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        AddHeaders(request, headers);

        var response = await _httpClient.SendAsync(request);
        return await HandleResponse<TResponse>(response);
    }

    public async Task<TResponse?> PutAsync<TResponse>(string endpoint, object? data = null, Dictionary<string, string>? headers = null)
    {
        return await PutAsync<object, TResponse>(endpoint, data!, headers);
    }
    public async Task<bool> DeleteAsync(string endpoint, Dictionary<string, string>? headers = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, endpoint);
        AddHeaders(request, headers);

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Request failed with statusCode '{response.StatusCode}': '{errorBody}'");
        }

        return true;
    }

    public async Task<TResponse?> SendAsync<TResponse>(HttpMethod method, string endpoint, string mediaType = "application/json", object? data = null, Dictionary<string, string>? headers = null)
    {
        var request = new HttpRequestMessage(method, endpoint);
        if (data != null)
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            request.Content = new StringContent(json, Encoding.UTF8, mediaType);
        }

        AddHeaders(request, headers);

        var response = await _httpClient.SendAsync(request);
        return await HandleResponse<TResponse>(response);
    }

    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
    {
        return await _httpClient.SendAsync(request);
    }

    private static void AddHeaders(HttpRequestMessage request, Dictionary<string, string>? headers)
    {
        if (headers == null) return;

        foreach (var header in headers)
        {
            request.Headers.Add(header.Key, header.Value);
        }
    }
    private async Task<TResponse?> HandleResponse<TResponse>(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Request failed with statusCode '{response.StatusCode}': '{errorBody}'");
        }

        var content = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrEmpty(content))
        {
            return default;
        }

        return JsonSerializer.Deserialize<TResponse>(content, _jsonOptions);
    }
}