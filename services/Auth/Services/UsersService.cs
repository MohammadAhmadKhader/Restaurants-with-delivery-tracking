using Auth.Models;
using Auth.Repositories.IRepositories;
using Auth.Services.IServices;
using Auth.Mappers;
using Auth.Utils;
using Shared.Exceptions;
using Auth.Contracts.Dtos.User;
using Shared.Data.Patterns.UnitOfWork;
using Auth.Data;

namespace Auth.Services;

public class UsersService(IUnitOfWork<AppDbContext> unitOfWork, IUsersRepository usersRepository) : IUsersService
{
    private readonly IUnitOfWork<AppDbContext> _unitOfWork = unitOfWork;
    private readonly IUsersRepository _usersRepository = usersRepository;
    private const string resourceName = "user";

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        var exists = await _usersRepository.ExistsByEmailAsync(email);
        if (exists)
        {
            throw new ConflictException("Email", email, ConflictType.Duplicate);
        }

        return exists;
    }

    public async Task<(List<User> users, int count)> FilterUsersAsync(UsersFilterParams filterParams)
    {
        return await _usersRepository.FilterUsersAsync(filterParams);
    }

    public async Task<User?> FindByEmailWithRolesAndPermissionsAsync(string email)
    {
        return await _usersRepository.FindByEmailWithRolesAndPermissionsAsync(email);;
    }

    public async Task<User?> FindByIdWithRolesAndPermissionsAsync(Guid id)
    {
        return await _usersRepository.FindByIdWithRolesAndPermissionsAsync(id);
    }

    public async Task<User?> FindByIdAsync(Guid id)
    {
        return await _usersRepository.FindByIdAsync(id);
    }

    public async Task<User> UpdateProfileAsync(Guid id, UserUpdateProfile dto)
    {
        var user = await _usersRepository.FindByIdAsync(id);
        if (user == null)
        {
            throw new ResourceNotFoundException(resourceName);
        }

        dto.PatchModel(user);

        await _unitOfWork.SaveChangesAsync();
        return user;
    }

    public async Task<User?> FindByIdWithRolesAsync(Guid id)
    {
        return await _usersRepository.FindByIdWithRolesAsync(id);
    }

    public async Task<(bool isSuccess, DeleteUserError error)> DeleteByIdAsync(Guid id)
    {
        var user = await _usersRepository.FindByIdWithRolesAsync(id);
        if (user == null)
        {
            return (false, DeleteUserError.NotFound);
        }

        if (user.Roles.Any(r => r.Name == RolePolicies.Admin))
        {
            return (false, DeleteUserError.ForbiddenAdmin);
        }

        if (user.Roles.Any(r => r.Name == RolePolicies.SuperAdmin))
        {
            return (false, DeleteUserError.ForbiddenOwner);
        }

        user.Email = null;
        user.NormalizedEmail = null;
        user.Roles.Clear();
        user.FirstName = null;
        user.LastName = null;
        user.UserName = null;
        user.NormalizedUserName = null;
        user.PasswordHash = null;
        user.PhoneNumber = null;
        user.IsDeleted = true;
        
        var changesCount = await _unitOfWork.SaveChangesAsync();
        if (changesCount < 1)
        {
            return (false, DeleteUserError.Unexpected);
        }

        return (true, DeleteUserError.None);
    }
}