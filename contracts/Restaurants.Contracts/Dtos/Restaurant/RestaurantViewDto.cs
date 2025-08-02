namespace Restaurants.Contracts.Dtos.Restaurant;

public class RestaurantViewDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}