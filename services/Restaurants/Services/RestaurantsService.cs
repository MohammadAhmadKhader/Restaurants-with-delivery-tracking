using Restaurants.Contracts.Dtos;
using Restaurants.Data;
using Restaurants.Mappers;
using Restaurants.Models;
using Restaurants.Repositories.IRepositories;
using Restaurants.Services.IServices;
using Shared.Data.Patterns.UnitOfWork;
using Shared.Exceptions;

namespace Restaurants.Services;

public class RestaurantsService(
    IUnitOfWork<AppDbContext> unitOfWork,
    IRestaurantInvitationsService restaurantInvitationsService,
    IRestaurantsRepository restaurantsRepository) : IRestaurantsService
{
    private readonly IUnitOfWork<AppDbContext> _unitOfWork = unitOfWork;
    private readonly IRestaurantInvitationsService _restaurantInvitationsService = restaurantInvitationsService;
    private readonly IRestaurantsRepository _restaurantsRepository = restaurantsRepository;

    public async Task<(List<Restaurant> restaurants, int count)> FindAllAsync(int page, int size)
    {
        return await _restaurantsRepository.FindAllOrderedDescAtAsync(page, size);
    }

    public async Task<Restaurant?> FindByIdAsync(Guid id)
    {
        return await _restaurantsRepository.FindByIdAsync(id);
    }

    public async Task<Restaurant> CreateAsync(RestaurantCreateDto dto, Guid token, Guid ownerId)
    {
        var exists = await _restaurantsRepository.ExistsByMatchAsync((rest) => rest.Name == dto.Name);
        if (exists)
        {
            throw new ConflictException($"restaurant with name '{dto.Name}' already exists", ConflictType.Duplicate);
        }

        using var tx = await _unitOfWork.BeginTransactionAsync();
        var inv = await _restaurantInvitationsService.MarkInvitationAsUsedAsync(token);

        var rest = dto.ToModel();
        rest.OwnerId = ownerId;

        var newResturat = await _restaurantsRepository.CreateAsync(rest);

        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitTransactionAsync(tx);

        return newResturat;
    }
}