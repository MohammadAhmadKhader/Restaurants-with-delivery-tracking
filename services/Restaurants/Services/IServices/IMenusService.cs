using Restaurants.Contracts.Dtos.Menu;
using Restaurants.Contracts.Dtos.MenuItems;
using Restaurants.Models;

namespace Restaurants.Services.IServices;

public interface IMenusService
{
    Task<(List<Menu> menus, int count)> FindAllAsync(int page, int size);
    Task<Menu?> FindByIdWithItemsAsync(int menuId);
    Task<Menu> CreateAsync(MenuCreateDto dto);
    Task<Menu> UpdateAsync(int menuId, MenuUpdateDto dto);
    Task DeleteAsync(int menuId);
    Task<Menu> AddItemsToMenuAsync(int menuId, MenuAddItemsDto dto);
    Task RemoveItemFromMenuAsync(int menuId, int menuItemId);

    Task<MenuItem> CreateItemAsync(MenuItemCreateDto dto);
    Task<MenuItem> UpdateItemAsync(int itemId, MenuItemUpdateDto dto);
    Task DeleteItemAsync(int menuId);
}