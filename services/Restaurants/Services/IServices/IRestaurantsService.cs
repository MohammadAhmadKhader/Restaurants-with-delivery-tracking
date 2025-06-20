using Restaurants.Contracts.Dtos;
using Restaurants.Models;

namespace Restaurants.Services.IServices;

public interface IRestaurantsService
{
    Task<(List<Restaurant> restaurants, int count)> FindAllAsync(int page, int size);
    Task<Restaurant?> FindByIdAsync(Guid id);
    Task<Restaurant> CreateAsync(RestaurantCreateDto dto, string? token);
}