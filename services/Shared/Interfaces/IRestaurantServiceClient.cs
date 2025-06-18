namespace Shared.Interfaces;
public interface IRestaurantServiceClient
{
    Task<object> TestPostAsync(object data);
}