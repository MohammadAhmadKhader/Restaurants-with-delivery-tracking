using Shared.Config;
using Shared.Contracts.Interfaces;
using Restaurants.Contracts.Clients;

namespace Shared.Http.Clients;

public class RestaurantServiceClient : BaseServiceClient, IRestaurantServiceClient
{
    private const string baseEndpoint = "/api/restaurants";
    public RestaurantServiceClient(IHttpClientService httpClientService) : base(httpClientService, MicroservicesUrlsProvider.Config.RestaurantsService)
    { }

    public async Task<object> TestPostAsync(object data)
    {
        var response = await _httpClientService.PostAsync<object>(_baseUrl + baseEndpoint + "/test", data);
        return response!;
    }
}