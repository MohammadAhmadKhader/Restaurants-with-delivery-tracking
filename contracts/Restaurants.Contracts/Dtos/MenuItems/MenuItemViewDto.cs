namespace Restaurants.Contracts.Dtos.MenuItems;

public class MenuItemViewDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
    public string ImageUrl { get; set; } = default!;
    public string ImagePublicId { get; set; } = default!;
}