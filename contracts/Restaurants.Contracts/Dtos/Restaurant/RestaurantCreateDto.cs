namespace Restaurants.Contracts.Dtos.Restaurant;

public class RestaurantCreateDto(string Name, string Description, string Phone)
{
    public string Name { get; set; } = Name?.ToLower()!;
    public string Description { get; set; } = Description;
    public string Phone { get; set; } = Phone!;
}