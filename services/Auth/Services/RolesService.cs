using Auth.Models;
using Auth.Repositories.IRepositories;
using Auth.Services;
using Auth.Services.IServices;

class RolesService(IUnitOfWork unitOfWork, IRolesRepository rolesRepository) : IRolesService
{
    public async Task<Role?> FindByIdAsync(Guid id)
    {
        return await rolesRepository.GetById(id);
    }

    public async Task<Role?> FindByName(sbyte name)
    {
        throw new NotImplementedException();
    }
}