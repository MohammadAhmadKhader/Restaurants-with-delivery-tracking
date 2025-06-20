namespace Restaurants.Contracts.Dtos;

public class RestaurantInvitationViewDto
{
    public Guid Id { get; set; }
    public Guid Token { get; set; }
    public string InvitedEmail { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
    public DateTime UsedAt { get; set; }
}