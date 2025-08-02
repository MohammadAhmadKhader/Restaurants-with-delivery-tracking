namespace Restaurants.Contracts.Dtos.Menu;

public class MenuCreateDto
{
    public string Category { get; set; } = default!;
    public string Name { get; set; } = default!;
}