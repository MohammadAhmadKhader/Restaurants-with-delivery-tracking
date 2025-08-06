using Refit;
using Restaurants.Contracts.Dtos.Menu;
using Restaurants.Contracts.Dtos.MenuItems;
using Shared.Contracts.Dtos;

namespace Restaurants.Contracts.Clients;

public static class MenusApiRoutes
{
    public const string BaseMenus = "/api/restaurants/menus";
    public const string BaseItems = "/api/restaurants/menus/items";
}

public interface IMenusServiceClient
{
    [Get(MenusApiRoutes.BaseMenus)]
    Task<CollectionResponse<MenuViewDto>> GetMenusAsync([Query] PagedRequest pagedRequest);

    [Get(MenusApiRoutes.BaseMenus + "/{id}")]
    Task<MenuResponseWrappaer> GetMenuByIdAsync(int id);

    [Get(MenusApiRoutes.BaseItems + "/{id}")]
    Task<MenuItemResponseWrappaer> GetMenuItemByIdAsync(int id);

    [Get(MenusApiRoutes.BaseItems + "/batch")]
    Task<MenuItemsResponseWrappaer> GetMenuItemsByIdAsync([Query(CollectionFormat.Multi)] List<int> ids);


    [Post(MenusApiRoutes.BaseMenus)]
    Task<MenuResponseWrappaer> CreateMenuAsync([Body] MenuCreateDto dto);

    [Post(MenusApiRoutes.BaseItems)]
    Task<MenuItemResponseWrappaer> CreateMenuItemAsync([Body] MenuItemCreateDto dto);


    [Put(MenusApiRoutes.BaseMenus + "/{id}")]
    Task<ApiResponse<object>> UpdateMenuAsync(int id, [Body] MenuUpdateDto dto);

    [Put(MenusApiRoutes.BaseItems + "/{id}")]
    Task<ApiResponse<object>> UpdateMenuItemAsync(int id, [Body] MenuItemUpdateDto dto);


    [Delete(MenusApiRoutes.BaseMenus + "/{id}")]
    Task<ApiResponse<object>> DeleteMenuAsync(int id);

    [Delete(MenusApiRoutes.BaseItems + "/{id}")]
    Task<ApiResponse<object>> DeleteMenuItemAsync(int id);


    [Patch(MenusApiRoutes.BaseMenus + "/{menuId}/items")]
    Task<ApiResponse<object>> AddItemsToMenuAsync(int menuId, [Body] MenuAddItemsDto dto);

    [Delete(MenusApiRoutes.BaseMenus + "/{menuId}/items/{itemId}")]
    Task<ApiResponse<object>> RemoveItemFromMenuAsync(int menuId, int itemId);
}

public class MenuResponseWrappaer
{
    public MenuViewDto Menu { get; set; } = default!;
}

public class MenuItemsResponseWrappaer
{
    public List<MenuItemViewDto> Items { get; set; } = default!;
}

public class MenuItemResponseWrappaer
{
    public MenuItemViewDto Item { get; set; } = default!;
}