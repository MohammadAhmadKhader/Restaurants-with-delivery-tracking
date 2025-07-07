using Auth.Contracts.Dtos.RestaurantRole;
using Auth.Models;

namespace Auth.Mappers;

public static class RestaurantRoleMappers
{
    public static RestaurantRoleWithPermissionViewDto ToViewWithPermissionsDto(this RestaurantRole role)
    {
        var dto = new RestaurantRoleWithPermissionViewDto
        {
            Id = role.Id,
            Name = role.NormalizedName,
            DisplayName = role.DisplayName,
            Permissions = role.Permissions.Select(p => p.ToViewDto()).ToList()
        };

        return dto;
    }

    public static RestaurantRoleViewDto ToViewDto(this RestaurantRole role)
    {
        var dto = new RestaurantRoleViewDto
        {
            Id = role.Id,
            Name = role.NormalizedName,
            DisplayName = role.DisplayName,
        };

        return dto;
    }

    public static void PatchModel(this RestaurantRoleUpdateDto dto, RestaurantRole role)
    {
        if (dto.Name != null)
        {
            role.NormalizedName = dto.Name.ToUpper();
        }

        if (dto.DisplayName != null)
        {
            role.DisplayName = dto.DisplayName;
        }
    }
}