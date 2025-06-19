using Auth.Contracts.Dtos;
using Auth.Contracts.Dtos.Role;
using Auth.Models;

namespace Auth.Mappers;

public static class RoleMappers
{
    public static RoleWithPermissionViewDto ToViewWithRolesAndPermissionsDto(this Role role)
    {
        var dto = new RoleWithPermissionViewDto
        {
            Id = role.Id,
            Name = role.Name,
            DisplayName = role.DisplayName,
            Permissions = role.Permissions.Select(p => p.ToViewDto()).ToList()
        };

        return dto;
    }

    public static RoleViewDto ToViewDto(this Role role)
    {
        var dto = new RoleViewDto
        {
            Id = role.Id,
            Name = role.Name,
            DisplayName = role.DisplayName,
        };

        return dto;
    }

    public static void PatchModel(this RoleUpdateDto dto, Role role)
    {
        if (dto.Name != null)
        {
            role.Name = dto.Name;
            role.NormalizedName = dto.Name.ToUpper();
        }

        if (dto.DisplayName != null)
        {
            role.DisplayName = dto.DisplayName;
        }
    }
}