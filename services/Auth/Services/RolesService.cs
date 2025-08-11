using Auth.Contracts.Dtos.Role;
using Auth.Models;
using Auth.Repositories.IRepositories;
using Auth.Services.IServices;
using Auth.Utils;
using Microsoft.AspNetCore.Identity;
using Shared.Exceptions;
using Auth.Mappers;
using Shared.Data.Patterns.UnitOfWork;
using Auth.Data;

namespace Auth.Services;

public class RolesService(
    IUnitOfWork<AppDbContext> unitOfWork,
    RoleManager<Role> roleManager,
    IRolesRepository rolesRepository,
    IPermissionsRepository permissionsRepository) : IRolesService
{
    private readonly IUnitOfWork<AppDbContext> _unitOfWork = unitOfWork;
    private readonly IRolesRepository _rolesRepository = rolesRepository;
    private readonly IPermissionsRepository _permissionsRepository = permissionsRepository;
    public const string resourceName = "role";
    public const string permissionResourceName = "permission";
    public async Task<Role?> FindByIdAsync(Guid id)
    {
        return await _rolesRepository.FindByIdAsync(id);
    }

    public async Task<Role?> FindByNameAsync(string name)
    {
        return await _rolesRepository.FindByNameAsync(name);
    }

    public async Task<(List<Role> roles, int count)> FindAllAsync(int page, int size)
    {
        var (roles, count) = await _rolesRepository.FindAllOrderedDescAtAsync(page, size);
        return (roles, count);
    }

    public async Task<Role> CreateAsync(RoleCreateDto dto)
    {
        var existedRole = await _rolesRepository.FindByNameOrDisplayNameAsync(dto.Name, dto.DisplayName);
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

        var role = new Role
        {
            Name = dto.Name,
            DisplayName = dto.DisplayName,
        };

        var result = await roleManager.CreateAsync(role);
        if (!result.Succeeded)
        {
            throw new InternalServerException();
        }

        return role;
    }

    public async Task DeleteAsync(Guid id)
    {
        var role = await roleManager.FindByIdAsync(id.ToString());
        ResourceNotFoundException.ThrowIfNull(role, resourceName);

        if (SecurityUtils.IsSuperAdminRole(role))
        {
            throw new InvalidOperationException("this role can not be deleted");
        }

        var result = await roleManager.DeleteAsync(role);
        if (!result.Succeeded)
        {
            throw new InternalServerException();
        }
    }

    public async Task<Role> UpdateAsync(Guid id, RoleUpdateDto dto)
    {
        var role = await roleManager.FindByIdAsync(id.ToString());
        ResourceNotFoundException.ThrowIfNull(role, resourceName);

        if (SecurityUtils.IsSuperAdminRole(role))
        {
            throw new InvalidOperationException("this role can not be modified");
        }

        dto.PatchModel(role);

        await _unitOfWork.SaveChangesAsync();

        return role;
    }

    public async Task<Role> AddPermissions(Guid roleId, List<int> permissionsIds)
    {
        var role = await _rolesRepository.FindByIdWithPermissionsAsync(roleId);
        ResourceNotFoundException.ThrowIfNull(role, resourceName);
        
        if (SecurityUtils.IsSuperAdminRole(role))
        {
            throw new InvalidOperationException("this role can not be modified");
        }

        var permissionsToAdd = await _permissionsRepository.FindByIds(permissionsIds);
        var fetchedIds = permissionsToAdd.Select(p => p.Id).ToHashSet();
        if (permissionsToAdd.Count != permissionsIds.Count)
        {
            var notFoundId = permissionsIds.First(id => !fetchedIds.Contains(id));

            throw new ResourceNotFoundException(permissionResourceName, notFoundId);
        }

        foreach (var permission in role.Permissions)
        {
            if (fetchedIds.Contains(permission.Id))
            {
                throw new ConflictException(permissionResourceName, permission.Name, ConflictType.AlreadyAssigned);
            }
        }

        foreach (var permission in permissionsToAdd)
        {
            var isOnlySuperAdminPermission = SecurityUtils.IsSuperAdminOnly(permission);
            if (isOnlySuperAdminPermission)
            {
                throw new InvalidOperationException("SuperAdmin Permission can not be added");
            }

            role.Permissions.Add(permission);
        }

        await _unitOfWork.SaveChangesAsync();
        return role;
    }

    public async Task<Role> RemovePermission(Guid roleId, int permissionId)
    {
        var role = await _rolesRepository.FindByIdWithPermissionsAsync(roleId);
        ResourceNotFoundException.ThrowIfNull(role, resourceName, roleId.ToString());

        if (SecurityUtils.IsSuperAdminRole(role))
        {
            throw new InvalidOperationException("this role can not be modified");
        }

        var permission = await _permissionsRepository.FindByIdAsync(permissionId);
        ResourceNotFoundException.ThrowIfNull(permission, permissionResourceName, permissionId);

        if (!role.Permissions.Contains(permission))
        {
            throw new ConflictException(permissionResourceName, permission.Name, ConflictType.NotAssigned);
        }

        role.Permissions.Remove(permission);

        await _unitOfWork.SaveChangesAsync();
        return role;
    }
}