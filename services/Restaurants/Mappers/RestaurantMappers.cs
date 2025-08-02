using Restaurants.Contracts.Dtos.Restaurant;
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
            CreatedAt = restaurant.CreatedAt,
        };
    }

    public static void PatchModel(this RestaurantUpdateDto dto, Restaurant model)
    {
        if (!string.IsNullOrWhiteSpace(dto.Name))
        {
            model.Name = dto.Name;
        }

        if (!string.IsNullOrWhiteSpace(dto.Phone))
        {
            model.Phone = dto.Phone;
        }

        if (!string.IsNullOrWhiteSpace(dto.Description))
        {
            model.Description = dto.Description;
        }
    }
}