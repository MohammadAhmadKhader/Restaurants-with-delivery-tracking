namespace Restaurants.Contracts.Clients;
public interface IRestaurantServiceClient
{
    Task<object> TestPostAsync(object data);
}