namespace Restaurants.Contracts.Dtos;

public class RestaurantUpdateDto(string Name, string Description, string Phone)
{
    public string? Name { get; set; } = Name?.ToLower();
    public string? Description { get; set; } = Description?.ToLower();
    public string? Phone { get; set; } = Phone;
}