namespace Restaurants.Contracts.Dtos.Restaurant;

public class RestaurantInvitationCreateDto(string Email)
{
    public string Email { get; set; } = Email?.ToLower()!;
}