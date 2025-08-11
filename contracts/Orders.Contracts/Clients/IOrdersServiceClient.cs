using Orders.Contracts.Dtos.Order;
using Refit;
using Shared.Contracts.Dtos;

namespace Orders.Contracts.Clients;

public interface IOrdersServiceClient
{
    [Get("/api/orders")]
    Task<CollectionResponse<OrderWithItemsCountViewDto>> GetOrdersAsync([Query] PagedRequest pagedRequest);

    [Get("/api/orders/{id}")]
    Task<OrderWithItemsResponseWrappaer> GetOrderByIdWihtItemsAsync(Guid id);

    [Post("/api/orders/place")]
    Task<OrderResponseWrappaer> PlaceOrderAsync([Body] OrderPlaceDto dto);

    [Patch("/api/orders/cancel/{id}")]
    Task<ApiResponse<object>> CancelOrderAsync(Guid id);
}

public class OrderWithItemsResponseWrappaer
{
    public OrderWithItemsViewDto Order { get; set; } = default!;
}

public class OrderResponseWrappaer
{
    public OrderViewDto Order { get; set; } = default!;
}