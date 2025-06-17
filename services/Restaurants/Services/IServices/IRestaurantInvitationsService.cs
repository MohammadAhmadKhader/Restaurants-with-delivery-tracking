using Restaurants.Models;

namespace Restaurants.Services.IServices;

public interface IRestaurantInvitationsService
{
    Task<RestaurantInvitation?> FindByIdAsync(Guid id);
    Task<RestaurantInvitation?> MarkInvitationAsUsedAsync(Guid id);
    Task<RestaurantInvitation?> CreateAsync(string email, Guid senderId);
    Task<bool> InvitiationExistsAsync(string token);
}