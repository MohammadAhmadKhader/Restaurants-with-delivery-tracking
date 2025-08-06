using Orders.Contracts.Dtos.Order;
using Orders.Mappers;
using Orders.Services.IServices;
using Shared.Auth;
using Shared.Contracts.Dtos;
using Shared.Filters;
using Shared.Utils;

namespace Orders.Endpoints;
public static class OrdersEndpoints
{
    public static void MapOrdersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders");

        group.MapGet("", async ([AsParameters] PagedRequest pagedReq, IOrdersService ordersService) =>
        {
            var (page, size) = PaginationUtils.NormalizeAndReturn(pagedReq);

            var (orders, count) = await ordersService.FindAllForCustomerWithItemsAsync(page, size);
            var ordersViews = orders.Select(x => x.ToViewWithItemsCountDto()).ToList();

            return Results.Ok(PaginationUtils.ResultOf(ordersViews, count, page, size));
        }).RequireAuthenticatedUser();

        group.MapGet("/{id}", async (Guid id, IOrdersService ordersService) =>
        {
            var order = await ordersService.FindByIdWithItemsAsync(id);
            if (order == null)
            {
                return ResponseUtils.NotFound("order");
            }

            return Results.Ok(new { order = order.ToViewWithItemsDto() });
        }).RequireAuthenticatedUser();

        group.MapPost("/place", async (OrderPlaceDto dto, IOrdersService ordersService) =>
        {
            var order = await ordersService.PlaceOrderAsync(dto);
            return Results.Ok(new { order = order.ToViewDto() });
        }).AddEndpointFilter<ValidationFilter<OrderPlaceDto>>()
        .RequireAuthenticatedUser();
        
        group.MapPatch("/cancel/{id}", async (Guid id, IOrdersService ordersService) =>
        {
            var order = await ordersService.MarkAsCancelledAsync(id);
            return Results.Ok(new { order =  order.ToViewDto() });
        }).RequireAuthenticatedUser();
    }
}