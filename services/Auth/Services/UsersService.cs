using Auth.Dtos.User;
using Auth.Models;
using Auth.Repositories.IRepositories;
using Auth.Services.IServices;
using Auth.Mappers;
using Auth.Utils;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Auth.Services;

public class UsersService(IUsersRepository usersRepository, IUnitOfWork unitOfWork) : IUsersService
{
    private readonly IUsersRepository _usersRepository = usersRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<bool> ExistsByEmail(string email)
    {
        return await _usersRepository.ExistsByEmail(email);
    }

    public async Task<(IReadOnlyList<User> users, int count)> FilterUsersAsync(UsersFilterParams filterParams)
    {
        return await _usersRepository.FilterUsersAsync(filterParams);
    }

    public async Task<User?> FindByEmailWithRolesAndPermissions(string email)
    {
        return await _usersRepository.FindByEmailWithRolesAndPermissions(email);
    }

    public async Task<User?> FindByIdWithRolesAndPermissions(Guid id)
    {
        return await _usersRepository.FindByIdWithRolesAndPermissions(id);
    }

    public async Task<User?> FindById(Guid id)
    {
        return await _usersRepository.FindById(id);
    }

    public async Task<User?> UpdateProfile(Guid id, UserUpdateProfile dto)
    {
        var user = await _usersRepository.FindById(id);
        if (user == null)
        {
            return null;
        }

        dto.PatchModel(user);

        await _unitOfWork.SaveChangesAsync();
        return user;
    }

    public async Task<User?> FindByIdWithRoles(Guid id)
    {
        return await _usersRepository.FindByIdWithRoles(id);
    }

    public async Task<(bool isSuccess, DeleteUserError error)> DeleteById(Guid id)
    {
        var user = await _usersRepository.FindByIdWithRoles(id);
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