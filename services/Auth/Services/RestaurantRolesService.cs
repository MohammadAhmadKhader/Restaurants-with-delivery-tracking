using Auth.Contracts.Dtos.RestaurantRole;
using Auth.Data;
using Auth.Mappers;
using Auth.Models;
using Auth.Repositories.IRepositories;
using Auth.Services.IServices;
using Auth.Utils;

using Shared.Data.Patterns.UnitOfWork;
using Shared.Exceptions;

namespace Auth.Services;

public class RestaurantRolesService(
    IUnitOfWork<AppDbContext> unitOfWork,
    IRestaurantRolesRepository rolesRepository,
    IRestaurantPermissionsRepository permissionsRepository,
    ITenantProvider tenantProvider) : IRestaurantRolesService
{
    private const string roleResourceName = "role";
    private const string permissionResourceName = "permission";
    public async Task<RestaurantRole?> FindByIdAsync(Guid id)
    {
        return await rolesRepository.FindByIdAsync(id);
    }

    public async Task<RestaurantRole?> FindByNameAsync(string name)
    {
        return await rolesRepository.FindByNameAsync(name);
    }
    
    public async Task<RestaurantRole?> FindByNameWithPermissionsAsync(string name)
    {
        return await rolesRepository.FindByNameWithPermissionsAsync(name);
    }
    public async Task<RestaurantRole> CreateAsync(RestaurantRoleCreateDto dto)
    {
        var existedRole = await rolesRepository.FindByNameOrDisplayNameAsync(dto.Name, dto.DisplayName);
        if (existedRole != null)
        {
            if (dto.DisplayName == existedRole.DisplayName)
            {
                throw new ConflictException("DisplayName", dto.DisplayName, ConflictType.Duplicate);
            }

            if (dto.Name.ToUpper() == existedRole.NormalizedName)
            {
                throw new ConflictException("Name", dto.Name, ConflictType.Duplicate);
            }

            // its not excpected to reach here but just in case
            throw new InternalServerException();
        }

        var role = new RestaurantRole
        {
            NormalizedName = dto.Name,
            DisplayName = dto.DisplayName,
            RestaurantId = tenantProvider.GetTenantIdOrThrow()
        };

        var newRole = await rolesRepository.CreateAsync(role);
        await unitOfWork.SaveChangesAsync();

        return newRole;
    }

    public async Task<RestaurantRole> UpdateAsync(Guid id, RestaurantRoleUpdateDto dto)
    {
        var role = await rolesRepository.FindByIdAsync(id);
        if (role == null)
        {
            throw new ResourceNotFoundException(roleResourceName);
        }

        if (role.NormalizedName == RolePolicies.RestaurantOwner)
        {
            throw new InvalidOperationException("this role can not be modified");
        }

        dto.PatchModel(role);

        await unitOfWork.SaveChangesAsync();

        return role;
    }

    public async Task DeleteAsync(Guid id)
    {
        var role = await rolesRepository.FindByIdAsync(id);
        if (role == null)
        {
            throw new ResourceNotFoundException(roleResourceName);
        }

        if (role.NormalizedName == RolePolicies.RestaurantOwner)
        {
            throw new InvalidOperationException("this role can not be deleted");
        }

        rolesRepository.Delete(role);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task<RestaurantRole> AddPermissions(Guid roleId, List<int> permissionsIds)
    {
        var role = await rolesRepository.FindByIdWithPermissionsAsync(roleId);
        if (role == null)
        {
            throw new ResourceNotFoundException(roleResourceName);
        }

        var permissionsToAdd = await permissionsRepository.FindManyByIdsAsync(permissionsIds);
        var fetchedIds = permissionsToAdd.Select(p => p.Id).ToHashSet();
        if (permissionsToAdd.Count != permissionsIds.Count)
        {
            var notFoundId = permissionsIds.First(id => !fetchedIds.Contains(id));

            throw new ResourceNotFoundException("permission", notFoundId);
        }

        foreach (var permission in role.Permissions)
        {
            if (fetchedIds.Contains(permission.Id))
            {
                throw new ConflictException("permission", permission.NormalizedName, ConflictType.AlreadyAssigned);
            }
        }

        foreach (var permission in permissionsToAdd)
        {
            var isOnlyOwnerPermission = SecurityUtils.IsOwnerOnly(permission);
            if (isOnlyOwnerPermission)
            {
                throw new InvalidOperationException("Owner Permission can not be added");
            }

            role.Permissions.Add(permission);
        }

        await unitOfWork.SaveChangesAsync();

        return role;
    }

    public async Task<RestaurantRole> RemovePermission(Guid roleId, int permissionId)
    {
        var role = await rolesRepository.FindByIdWithPermissionsAsync(roleId);
        if (role == null)
        {
            throw new ResourceNotFoundException(roleResourceName);
        }

        if (role.NormalizedName == RolePolicies.RestaurantOwner)
        {
            throw new InvalidOperationException("this role can not be modified");
        }

        var permission = await permissionsRepository.FindByIdAsync(permissionId);
        if (permission == null)
        {
            throw new ResourceNotFoundException(permissionResourceName);
        }

        if (!role.Permissions.Contains(permission))
        {
            throw new ConflictException("permission", permission.NormalizedName, ConflictType.NotAssigned);
        }

        role.Permissions.Remove(permission);
        await unitOfWork.SaveChangesAsync();

        return role;
    }
}