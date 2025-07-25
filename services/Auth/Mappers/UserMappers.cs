using Auth.Contracts.Dtos.User;
using Auth.Models;

namespace Auth.Mappers;

public static class UserMappers
{
    public static UserViewDto ToViewDto(this User user)
    {
        var dto = new UserViewDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };

        return dto;
    }

    public static UserWithRolesAndPermissionsDto ToViewWithRolesAndPermissionsDto(this User user)
    {
        var dto = new UserWithRolesAndPermissionsDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            Roles = user.Roles.Select(r => r.ToViewWithPermissionsDto()).ToList()
        };

        return dto;
    }

    public static UserWithRestaurantRolesAndPermissionsDto ToViewWithRestaurantRolesAndPermissionsDto(this User user)
    {
        var dto = new UserWithRestaurantRolesAndPermissionsDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            Roles = user.RestaurantRoles.Select(r => r.ToViewWithPermissionsDto()).ToList()
        };

        return dto;
    }

    public static UserWithRolesDto ToViewWithRolesDto(this User user)
    {
        var dto = new UserWithRolesDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            Roles = user.Roles.Select(r => r.ToViewDto()).ToList()
        };

        return dto;
    }

    public static void PatchModel(this UserUpdateProfile dto, User user)
    {
        if (dto.FirstName != null)
        {
            user.FirstName = dto.FirstName;
        }

        if (dto.LastName != null)
        {
            user.LastName = dto.LastName;
        }
    }
}