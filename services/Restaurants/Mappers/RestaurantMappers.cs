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
}