namespace Shared.Interfaces;

public interface IHttpClientService
{
    Task<TResponse?> GetAsync<TResponse>(string endpoint, Dictionary<string, string>? headers = null);
    Task<TResponse?> PostAsync<TResponse>(string endpoint, object? data = null, Dictionary<string, string>? headers = null);
    Task<TResponse?> PutAsync<TResponse>(string endpoint, object? data = null, Dictionary<string, string>? headers = null);
    Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest data, Dictionary<string, string>? headers = null);
    Task<bool> DeleteAsync(string endpoint, Dictionary<string, string>? headers = null);
    Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data, Dictionary<string, string>? headers = null);

    /// <summary>
    /// just in case a custom requirement was required.
    /// </summary>
    Task<TResponse?> SendAsync<TResponse>(HttpMethod method, string endpoint, string mediaType = "application/json", object? data = null, Dictionary<string, string>? headers = null);

    /// <summary>
    /// for more control (most likely shouldnt be used) but just incase.
    /// </summary>
    Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
}