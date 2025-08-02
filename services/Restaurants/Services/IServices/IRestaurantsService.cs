using Restaurants.Contracts.Dtos.Restaurant;
using Restaurants.Models;

namespace Restaurants.Services.IServices;

public interface IRestaurantsService
{
    Task<(List<Restaurant> restaurants, int count)> FindAllAsync(int page, int size);
    Task<Restaurant?> FindByIdAsync(Guid id);
    Task<Restaurant> CreateAsync(RestaurantInvitationAcceptDto invitationDto);
    Task<Restaurant> UpdateAsync(RestaurantUpdateDto dto);
}