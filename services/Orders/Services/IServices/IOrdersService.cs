using Orders.Contracts.Dtos.Order;
using Orders.Contracts.Enums;
using Orders.Models;

namespace Orders.Services.IServices;

public interface IOrdersService
{
    Task<Order?> FindByIdAsync(Guid id);
    Task<Order?> FindByIdWithItemsAsync(Guid id);
    Task<(List<Order>, int count)> FindAllForCustomerWithItemsAsync(int page, int size);
    Task<Order?> FindByIdForCustomerWithItemsAsync(Guid id);
    Task<Order> PlaceOrderAsync(OrderPlaceDto dto);
    Task<Order> MarkAsCancelledAsync(Guid id);
    Task<Order> MarkAsPayedAsync(Guid id);
    Task<Order> UpdateStatusAsync(Guid id, OrderStatus newStatus);
}