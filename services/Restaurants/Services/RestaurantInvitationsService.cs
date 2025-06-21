using Restaurants.Data;
using Restaurants.Models;
using Restaurants.Repositories.IRepositories;
using Restaurants.Services.IServices;
using Shared.Data.Patterns.UnitOfWork;
using Shared.Exceptions;

namespace Restaurants.Services;

public class RestaurantInvitationsService(
    IUnitOfWork<AppDbContext> unitOfWork,
    IRestaurantInvitationsRepository restaurantInvitationsRepository) : IRestaurantInvitationsService
{
    public const int InvitiationLifetimeInDays = 3;
    private readonly IUnitOfWork<AppDbContext> _unitOfWork = unitOfWork;
    private const string _resourceName = "restaurant-invitation";
    private readonly IRestaurantInvitationsRepository _restaurantInvitationsRepository = restaurantInvitationsRepository;

    public async Task<RestaurantInvitation?> FindByIdAsync(Guid id)
    {
        return await _restaurantInvitationsRepository.FindByIdAsync(id);
    }

    public async Task<RestaurantInvitation> MarkInvitationAsUsedAsync(Guid id)
    {
        var invitiation = await _restaurantInvitationsRepository.FindByIdAsync(id);
        if (invitiation == null)
        {
            throw new ResourceNotFoundException(_resourceName);
        }

        if (invitiation.ExpiresAt <= DateTime.UtcNow)
        {
            throw new InvalidOperationException("expired invitation");
        }

        if (invitiation.UsedAt != null)
        {
            throw new InvalidOperationException("invitation was used already");
        }

        invitiation.UsedAt = DateTime.UtcNow;
        
        await _unitOfWork.SaveChangesAsync();

        return invitiation;
    }

    public async Task<RestaurantInvitation> CreateAsync(string email, Guid senderId)
    {
        var newToken = Guid.NewGuid();
        var invitiation = new RestaurantInvitation
        {
            InvitedById = senderId,
            InvitedEmail = email,
            Token = newToken,
            ExpiresAt = DateTime.UtcNow.AddDays(InvitiationLifetimeInDays),
        };

        var createdInvitation = await _restaurantInvitationsRepository.CreateAsync(invitiation);
        return createdInvitation;
    }

    public async Task<bool> InvitiationExistsAsync(string token)
    {
        var isSuccess = Guid.TryParse(token, out var guidToken);
        if (!isSuccess)
        {
            return false;
        }

        return await _restaurantInvitationsRepository
        .ExistsByMatchAsync((inv) => inv.Token == guidToken && inv.ExpiresAt > DateTime.UtcNow);
    }
}