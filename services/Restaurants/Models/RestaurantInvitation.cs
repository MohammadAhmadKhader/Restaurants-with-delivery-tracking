using System.ComponentModel.DataAnnotations;

namespace Restaurants.Models;

public class RestaurantInvitation
{
    [Key]
    public Guid Id { get; set; }
    public Guid Token { get; set; }
    public string InvitedEmail { get; set; } = default!;
    public Guid InvitedById { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? UsedAt { get; set; }
}