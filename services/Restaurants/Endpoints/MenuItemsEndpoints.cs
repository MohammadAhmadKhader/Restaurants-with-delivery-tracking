using Restaurants.Contracts.Dtos.MenuItems;
using Restaurants.Mappers;
using Restaurants.Services.IServices;
using Shared.Auth;
using Shared.Constants;

namespace Restaurants.Endpoints;
public static class MenuItemsEndpoints
{
    public static void MapMenuItemsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/restaurants/menus/items");

        group.MapPost("", async (MenuItemCreateDto dto, IMenusService menuItemsService) =>
        {
            var newItem = await menuItemsService.CreateItemAsync(dto);

            return Results.Ok(new { item = newItem.ToViewDto() });
        }).RequirePermission(RestaurantPermissions.RESTAURANT_MENU_ITEMS_CREATE);

        group.MapPut("/{id}", async (int id, MenuItemUpdateDto dto, IMenusService menuItemsService) =>
        {
            var newItem = await menuItemsService.UpdateItemAsync(id, dto);

            return Results.NoContent();
        }).RequirePermission(RestaurantPermissions.RESTAURANT_MENU_ITEMS_CREATE);

        group.MapDelete("/{id}", async (int id, IMenusService menuItemsService) =>
        {
            await menuItemsService.DeleteItemAsync(id);

            return Results.NoContent();
        }).RequirePermission(RestaurantPermissions.RESTAURANT_MENU_ITEMS_DELETE);
    }
}