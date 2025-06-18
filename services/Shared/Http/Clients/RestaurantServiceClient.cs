using Shared.Config;
using Shared.Interfaces;

namespace Shared.Http.Clients;

public class RestaurantServiceClient : BaseServiceClient, IRestaurantServiceClient
{
    public RestaurantServiceClient(IHttpClientService httpClientService) :base(httpClientService, MicroservicesUrlsProvider.Config.RestaurantsService)
    { }

    public async Task<object> TestPostAsync(object data)
    {
        var response = await _httpClientService.PostAsync<object>(_baseUrl+"/api/restaurants/test", data);
        await _httpClientService.PostAsync<object>(_baseUrl+"/api/restaurants/test", data);
        return response!;
    }
}