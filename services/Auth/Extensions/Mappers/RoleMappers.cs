using Auth.Dtos;
using Auth.Models;

namespace Auth.Extensions.Mappers;

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
}