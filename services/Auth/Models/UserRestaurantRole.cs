namespace Auth.Models;

public class UserRestaurantRole
{
    public Guid UserId { get; set; }
    public Guid RestaurantId { get; set; }
}