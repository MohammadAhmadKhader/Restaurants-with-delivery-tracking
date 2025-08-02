using Restaurants.Contracts.Dtos.MenuItems;

namespace Restaurants.Contracts.Dtos.Menu;
public class MenuWithItemsViewDto: MenuViewDto
{
    public List<MenuItemViewDto> Items { get; set; } = [];
}