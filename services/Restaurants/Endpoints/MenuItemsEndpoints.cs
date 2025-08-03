using Microsoft.AspNetCore.Mvc;
using Restaurants.Contracts.Dtos.MenuItems;
using Restaurants.Mappers;
using Restaurants.Services.IServices;
using Shared.Auth;
using Shared.Constants;
using Shared.Utils;

namespace Restaurants.Endpoints;
public static class MenuItemsEndpoints
{
    public static void MapMenuItemsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/restaurants/menus/items");

        group.MapGet("/{id}", async (int id, IMenusService menusService) =>
        {
            var item = await menusService.FindItemByIdAsync(id);
            if (item == null)
            {
                return ResponseUtils.NotFound("menu-item");
            }

            return Results.Ok(new { item = item.ToViewDto() });
        });

        group.MapPost("", async ([FromForm] MenuItemCreateDto dto, IMenusService menusService) =>
        {
            var newItem = await menusService.CreateItemAsync(dto);

            return Results.Ok(new { item = newItem.ToViewDto() });
        })
        .DisableAntiforgery()
        .RequirePermission(RestaurantPermissions.RESTAURANT_MENU_ITEMS_CREATE);

        group.MapPut("/{id}", async (int id, [FromForm] MenuItemUpdateDto dto, IMenusService menusService) =>
        {
            var newItem = await menusService.UpdateItemAsync(id, dto);

            return Results.NoContent();
        })
        .DisableAntiforgery()
        .RequirePermission(RestaurantPermissions.RESTAURANT_MENU_ITEMS_CREATE);

        group.MapDelete("/{id}", async (int id, IMenusService menusService) =>
        {
            await menusService.DeleteItemAsync(id);

            return Results.NoContent();
        }).RequirePermission(RestaurantPermissions.RESTAURANT_MENU_ITEMS_DELETE);
    }
}