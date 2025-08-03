using Microsoft.AspNetCore.Http;

namespace Restaurants.Contracts.Dtos.MenuItems;

public class MenuItemCreateDto
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal Price { get; set; }
    public bool? IsAvailable { get; set; }
    public IFormFile Image { get; set; } = default!;
}