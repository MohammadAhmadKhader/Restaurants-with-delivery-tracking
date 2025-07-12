using MassTransit;
using Microsoft.Extensions.Caching.Hybrid;
using Restaurants.Contracts.Dtos;
using Restaurants.Data;
using Restaurants.Mappers;
using Restaurants.Models;
using Restaurants.Repositories.IRepositories;
using Restaurants.Services.IServices;
using Shared.Data.Patterns.UnitOfWork;
using Shared.Exceptions;
using Shared.Kafka;

namespace Restaurants.Services;

public class RestaurantsService(
    IUnitOfWork<AppDbContext> unitOfWork,
    IRestaurantInvitationsService restaurantInvitationsService,
    IRestaurantsRepository restaurantsRepository,
    ILogger<RestaurantsService> logger,
    ITopicProducer<RestaurantCreatedEvent> restaurantCreatedProducer,
    HybridCache cache) : IRestaurantsService
{
    private readonly IUnitOfWork<AppDbContext> _unitOfWork = unitOfWork;
    private readonly IRestaurantInvitationsService _restaurantInvitationsService = restaurantInvitationsService;
    private readonly IRestaurantsRepository _restaurantsRepository = restaurantsRepository;
    private readonly ILogger<RestaurantsService> _logger = logger;
    private readonly ITopicProducer<RestaurantCreatedEvent> _restaurantCreatedProducer = restaurantCreatedProducer;
    private readonly HybridCache _cache = cache;

    public async Task<(List<Restaurant> restaurants, int count)> FindAllAsync(int page, int size)
    {
        return await _restaurantsRepository.FindAllOrderedDescAtAsync(page, size);
    }

    public async Task<Restaurant?> FindByIdAsync(Guid id)
    {
        return await _cache.GetOrCreateAsync("restaurants:" + id, async(token) =>
        {
            return await _restaurantsRepository.FindByIdAsync(id);
        });
    }

    public async Task<Restaurant> CreateAsync(RestaurantInvitationAcceptDto invDto)
    {
        var ownerId = Guid.CreateVersion7();

        var restDto = invDto.Restaurant;
        var exists = await _restaurantsRepository.ExistsByMatchAsync((rest) => rest.Name == restDto.Name);
        if (exists)
        {
            throw new ConflictException($"restaurant with name '{restDto.Name}' already exists", ConflictType.Duplicate);
        }

        using var tx = await _unitOfWork.BeginTransactionAsync();
        var inv = await _restaurantInvitationsService.MarkInvitationAsUsedAsync(invDto.InvitationId);

        var rest = restDto.ToModel();
        rest.OwnerId = ownerId;

        var newResturat = await _restaurantsRepository.CreateAsync(rest);

        await _unitOfWork.SaveChangesAsync();

        var ev = new RestaurantCreatedEvent(
            invDto.InvitationId,
            newResturat.Id,
            ownerId,
            invDto.User
        );

        _logger.LogInformation("Sending event {@RestaurantCreatedEvent}", ev);
        await _restaurantCreatedProducer.Produce(ev);

        await _unitOfWork.CommitTransactionAsync(tx);

        return newResturat;
    }
}