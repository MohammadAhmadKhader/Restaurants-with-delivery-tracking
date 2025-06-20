using Restaurants.Models;
using Restaurants.Repositories.IRepositories;
using Restaurants.Services.IServices;
using Shared.Exceptions;

namespace Restaurants.Services;

public class RestaurantInvitationsService(IUnitOfWork unitOfWork) : IRestaurantInvitationsService
{
    public const int InvitiationLifetimeInDays = 3;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private const string _resourceName = "restaurant-invitation";

    public async Task<RestaurantInvitation?> FindByIdAsync(Guid id)
    {
        return await _unitOfWork.RestaurantInvitationsRepository.FindByIdAsync(id);
    }

    public async Task<RestaurantInvitation?> MarkInvitationAsUsedAsync(Guid id)
    {
        var invitiation = await _unitOfWork.RestaurantInvitationsRepository.FindByIdAsync(id);
        if (invitiation == null)
        {
            throw new ResourceNotFoundException(_resourceName);
        }

        if (invitiation.ExpiresAt <= DateTime.UtcNow)
        {
            throw new InvalidOperationException("expired invitation");
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

        var createdInvitation = await _unitOfWork.RestaurantInvitationsRepository.CreateAsync(invitiation);
        return createdInvitation;
    }

    public async Task<bool> InvitiationExistsAsync(string token)
    {
        var isSuccess = Guid.TryParse(token, out var guidToken);
        if (!isSuccess)
        {
            return false;
        }

        return await _unitOfWork.RestaurantInvitationsRepository
        .ExistsByMatchAsync((inv) => inv.Token == guidToken && inv.ExpiresAt > DateTime.UtcNow);
    }
}