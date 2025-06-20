using Restaurants.Contracts.Dtos;
using Restaurants.Models;

namespace Restaurants.Mappers;

public static class RestaurantMappers
{
    public static Restaurant ToModel(this RestaurantCreateDto dto)
    {
        return new Restaurant
        {
            Name = dto.Name,
            Description = dto.Description,
            Phone = dto.Phone
        };
    }

    public static RestaurantViewDto ToViewDto(this Restaurant restaurant)
    {
        return new RestaurantViewDto
        {
            Id = restaurant.Id,
            Name = restaurant.Name,
            Description = restaurant.Description,
            Phone = restaurant.Phone,
            IsOpen = restaurant.IsOpen,
            CreatedAt = restaurant.CreatedAt,
        };
    }
}