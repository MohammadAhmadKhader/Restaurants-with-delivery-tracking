using Shared.Contracts.Attributes;

namespace Restaurants.Contracts.Dtos.Restaurant;

public class RestaurantInvitationCreateDto(string Email)
{
    [Masked]
    public string Email { get; set; } = Email?.ToLower()!;
}