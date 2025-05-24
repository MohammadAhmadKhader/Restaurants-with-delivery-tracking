using Auth.Dtos;
using Auth.Models;
using Auth.Repositories.IRepositories;
using Auth.Services.IServices;

namespace Auth.Services;

public class UsersService(IUsersRepository usersRepository) : IUsersService
{
    private readonly IUsersRepository _usersRepository = usersRepository;

    public async Task<bool> ExistsByEmail(string email)
    {
        return await _usersRepository.ExistsByEmail(email);
    }

    public async Task<User?> FindByEmailWithRolesAndPermissions(string email)
    {
        return await _usersRepository.FindByEmailWithRolesAndPermissions(email);
    }

    public async Task<User?> FindById(Guid Id)
    {
        return await _usersRepository.GetById(Id);
    }
}