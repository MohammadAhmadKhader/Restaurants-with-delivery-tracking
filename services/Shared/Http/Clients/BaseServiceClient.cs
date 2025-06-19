using Shared.Contracts.Interfaces;

namespace Shared.Http.Clients;

public class BaseServiceClient
{
    protected readonly IHttpClientService _httpClientService;
    protected readonly string _baseUrl;
    public BaseServiceClient(IHttpClientService httpClientService, string baseUrl)
    {
        _httpClientService = httpClientService;
        _baseUrl = baseUrl;
    }

    protected string BuildUrl(string endpoint) => $"{_baseUrl}/{endpoint.TrimEnd()}";
}