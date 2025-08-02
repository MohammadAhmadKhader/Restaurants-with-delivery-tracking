namespace Restaurants.Contracts.Dtos.Restaurant;

public class RestaurantInvitationViewDto
{
    public Guid Id { get; set; }
    public string InvitedEmail { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
    public DateTime? UsedAt { get; set; }
}