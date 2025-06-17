using Restaurants.Repositories.IRepositories;
using Restaurants.Services.IServices;

namespace Restaurants.Services;
public class MenusService(IUnitOfWork unitOfWork) : IMenusService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
}