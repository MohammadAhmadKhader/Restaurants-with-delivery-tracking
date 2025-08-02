namespace Restaurants.Contracts.Dtos.Menu;
public class MenuViewDto
{
    public int Id { get; set; }
    public Guid RestaurantId { get; set; }
    public string Category { get; set; } = default!;
    public string Name { get; set; } = default!;
}