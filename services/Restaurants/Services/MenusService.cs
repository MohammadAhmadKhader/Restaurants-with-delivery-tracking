using Restaurants.Data;
using Restaurants.Repositories.IRepositories;
using Restaurants.Services.IServices;
using Shared.Data.Patterns.UnitOfWork;

namespace Restaurants.Services;
public class MenusService(IUnitOfWork<AppDbContext> unitOfWork) : IMenusService
{
    private readonly IUnitOfWork<AppDbContext> _unitOfWork = unitOfWork;
}