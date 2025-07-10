using Auth.Contracts.Dtos.Auth;

namespace Restaurants.Contracts.Dtos;

public class RestaurantInvitationAcceptDto
{
    public Guid InvitationId { get; set; }
    public RestaurantCreateDto Restaurant { get; set; } = default!;
    public RegisterDto User { get; set; } = default!;
}