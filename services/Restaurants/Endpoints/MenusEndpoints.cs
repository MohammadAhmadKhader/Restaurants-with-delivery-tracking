using Restaurants.Contracts.Dtos.Menu;
using Restaurants.Mappers;
using Restaurants.Services.IServices;
using Shared.Auth;
using Shared.Constants;
using Shared.Contracts.Dtos;
using Shared.Utils;

namespace Restaurants.Endpoints;

public static class MenusEndpoints
{
    public static void MapMenusEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/restaurants/menus");

        group.MapGet("", async ([AsParameters] PagedRequest pagedReq, IMenusService menusService) =>
        {
            var (page, size) = PaginationUtils.NormalizeAndReturn(pagedReq);

            var (menus, count) = await menusService.FindAllAsync(page, size);
            var menusViews = menus.Select(r => r.ToViewDto()).ToList();

            return Results.Ok(PaginationUtils.ResultOf(menusViews, count, page, size));
        });

        group.MapGet("/{id}", async (int id, IMenusService menusService) =>
        {
            var menu = await menusService.FindByIdWithItemsAsync(id);
            if (menu == null)
            {
                return ResponseUtils.NotFound("menu");
            }

            return Results.Ok(new { menu = menu.ToViewWithItemsDto() });
        });

        group.MapPost("/", async (MenuCreateDto dto, IMenusService menusService) =>
        {
            var menu = await menusService.CreateAsync(dto);

            return Results.Ok(new { menu = menu.ToViewDto() });
        }).RequirePermission(RestaurantPermissions.RESTAURANT_MENUS_CREATE);

        group.MapPost("/{id}/items", async (int id, MenuAddItemsDto dto, IMenusService menusService) =>
        {
            var menu = await menusService.AddItemsToMenuAsync(id, dto);

            return Results.NoContent();
        }).RequirePermission(RestaurantPermissions.RESTAURANT_MENUS_ADD_ITEM);

        group.MapDelete("/{id}/items/{itemId}", async (int id, int itemId, IMenusService menusService) =>
        {
            await menusService.RemoveItemFromMenuAsync(id, itemId);

            return Results.NoContent();
        }).RequirePermission(RestaurantPermissions.RESTAURANT_MENUS_REMOVE_ITEM);

        group.MapPut("/{id}", async (int id, MenuUpdateDto dto, IMenusService menusService) =>
        {
            var menu = await menusService.UpdateAsync(id, dto);

            return Results.NoContent();
        }).RequirePermission(RestaurantPermissions.RESTAURANT_MENUS_UPDATE);
        
        group.MapDelete("/{id}", async (int id, IMenusService menusService) =>
        {
            await menusService.DeleteAsync(id);
            
            return Results.NoContent();
        }).RequirePermission(RestaurantPermissions.RESTAURANT_MENUS_DELETE);
    }
}