namespace Restaurants.Dtos;

public class RestaurantInvitationCreateDto(string Email)
{
    public string Email { get; set; } = Email?.ToLower()!;
}