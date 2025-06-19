using Auth.Contracts.Dtos.Permission;
using Auth.Models;

namespace Auth.Mappers;

public static class PermissionMappers
{
    public static PermissionViewDto ToViewDto(this Permission permission)
    {
        var dto = new PermissionViewDto
        {
            Id = permission.Id,
            Name = permission.Name,
            DisplayName = permission.DisplayName,
        };

        return dto;
    }
}