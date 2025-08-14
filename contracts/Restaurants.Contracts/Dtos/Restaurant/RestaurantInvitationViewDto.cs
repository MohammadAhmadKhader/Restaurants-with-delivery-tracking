using Shared.Contracts.Attributes;

namespace Restaurants.Contracts.Dtos.Restaurant;

public class RestaurantInvitationViewDto
{
    public Guid Id { get; set; }

    [Masked]
    public string InvitedEmail { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
    public DateTime? UsedAt { get; set; }
}