using Auth.Contracts.Dtos.RestaurantPermission;
using Auth.Models;

namespace Auth.Mappers;
public static class RestaurantPermissionMappers
{
    public static RestaurantPermissionViewDto ToViewDto(this RestaurantPermission permission)
    {
        var dto = new RestaurantPermissionViewDto
        {
            Id = permission.Id,
            Name = permission.NormalizedName,
            DisplayName = permission.DisplayName,
        };

        return dto;
    }
}