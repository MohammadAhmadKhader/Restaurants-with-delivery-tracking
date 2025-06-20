using Restaurants.Contracts.Dtos;
using Restaurants.Mappers;
using Restaurants.Models;
using Restaurants.Repositories.IRepositories;
using Restaurants.Services.IServices;

namespace Restaurants.Services;

public class RestaurantsService(
    IUnitOfWork unitOfWork,
    IRestaurantInvitationsService restaurantInvitationsService) : IRestaurantsService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IRestaurantInvitationsService _restaurantInvitationsService = restaurantInvitationsService;

    public async Task<(List<Restaurant> restaurants, int count)> FindAllAsync(int page, int size)
    {
        return await _unitOfWork.RestaurantsRepository.FindAllOrderedDescAtAsync(page, size);
    }

    public async Task<Restaurant?> FindByIdAsync(Guid id)
    {
        return await _unitOfWork.RestaurantsRepository.FindByIdAsync(id);
    }

    public async Task<Restaurant> CreateAsync(RestaurantCreateDto dto, string? token)
    {
        var isSuccess = Guid.TryParse(token, out var guidToken);
        if (!isSuccess)
        {
            throw new InvalidOperationException("invalid invitation token");
        }

        var inv = await _restaurantInvitationsService.MarkInvitationAsUsedAsync(guidToken);

        // TODO:
        // - we make user register here through a link as well (we need to call Auth Service)

        var rest = dto.ToModel();
        rest.OwnerId = Guid.NewGuid();
        return await _unitOfWork.RestaurantsRepository.CreateAsync(rest);
    }
}