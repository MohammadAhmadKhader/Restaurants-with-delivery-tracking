using Orders.Contracts.Dtos.Order;
using Orders.Contracts.Enums;
using Orders.Data;
using Orders.Mappers;
using Orders.Models;
using Orders.Repositories.IRepositories;
using Orders.Services.IServices;
using Restaurants.Contracts.Dtos.MenuItems;
using Shared.Common;
using Shared.Contracts.Interfaces;
using Shared.Data.Patterns.UnitOfWork;
using Shared.Exceptions;

namespace Orders.Services;

public class OrdersService(
    IOrdersRepository ordersRepository,
    IUnitOfWork<AppDbContext> unitOfWork,
    IAuthProvider authProvider,
    IResourceBatchRetriever<int, MenuItemViewDto> resourceBatchRetriever
    ) : IOrdersService
{
    private readonly IOrdersRepository _ordersRepository = ordersRepository;
    private readonly IUnitOfWork<AppDbContext> _unitOfWork = unitOfWork;
    private readonly IAuthProvider _authProvider = authProvider;
    private readonly IResourceBatchRetriever<int, MenuItemViewDto> _resourceBatchRetriever = resourceBatchRetriever;
    private const string _resourceName = "order";
    public async Task<(List<Order>, int)> FindAllForCustomerWithItemsAsync(int page, int size)
        => await _ordersRepository.FindAllForCustomerWithItemsAsync(page, size, _authProvider.UserInfo.UserId);
    public async Task<Order?> FindByIdForCustomerWithItemsAsync(Guid id)
        => await _ordersRepository.FindByIdForCustomerWithItemsAsync(id, _authProvider.UserInfo.UserId);

    public async Task<Order?> FindByIdAsync(Guid id) => await _ordersRepository.FindByIdAsync(id);
    public async Task<Order?> FindByIdWithItemsAsync(Guid id)  => await _ordersRepository.FindByIdWithItemsAsync(id);

    public async Task<Order> PlaceOrderAsync(OrderPlaceDto dto)
    {
        var menuItemsById = await _resourceBatchRetriever.ValidateAndRetrieveAsDictAsync(
            dto.Items.Select( x=> x.ItemId),
            "item");

        decimal totalAmount = dto.Items
            .Sum(i => menuItemsById[i.ItemId].Price * i.Quantity);

        var model = dto.ToModel();

        model.CustomerId = _authProvider.UserInfo.UserId;
        model.RestaurantId = _authProvider.UserInfo.RestaurantId!.Value;
        model.TotalAmount = totalAmount;

        foreach (var item in dto.Items)
        {
            model.Items.Add(new OrderItem
            {
                MenuItemId = item.ItemId,
                Quantity = item.Quantity,
                Price = menuItemsById[item.ItemId].Price
            });
        }

        var newOrder = await _ordersRepository.CreateAsync(model);
        await _unitOfWork.SaveChangesAsync();
        
        return newOrder;
    }
    
    public async Task<Order> MarkAsCancelledAsync(Guid id)
    {
        var order = await _ordersRepository.FindByIdAsync(id);
        ResourceNotFoundException.ThrowIfNull(order, _resourceName);
        ThrowIfNotOwenr(order, _authProvider.UserInfo.UserId);

        if (order.Status == OrderStatus.Delivered)
        {
            throw new InvalidOperationException("Can not cancel of a delivered order.");
        }

        if (order.Status == OrderStatus.Cancelled)
        {
            throw new InvalidOperationException("Order is already cancelled.");
        }

        order.Status = OrderStatus.Cancelled;
        order.CancelledAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();

        return order;
    }

    public async Task<Order> UpdateStatusAsync(Guid id, OrderStatus newStatus)
    {
        var order = await _ordersRepository.FindByIdAsync(id);
        ResourceNotFoundException.ThrowIfNull(order, _resourceName);
        ThrowIfNotOwenr(order, _authProvider.UserInfo.UserId);

        if (order.Status == newStatus)
        {
            throw new InvalidOperationException("Can not re-set order to the same status.");
        }

        if (order.Status == OrderStatus.Delivered)
        {
            throw new InvalidOperationException("Can not change status of a delivered order.");
        }

        order.Status = newStatus;
        if (newStatus == OrderStatus.Delivered)
        {
            order.DeliveredAt = DateTime.UtcNow;
        }

        await _unitOfWork.SaveChangesAsync();

        return order;
    }


    public async Task<Order> MarkAsPayedAsync(Guid id)
    {
        var order = await _ordersRepository.FindByIdAsync(id);
        ResourceNotFoundException.ThrowIfNull(order, _resourceName);

        order.Status = OrderStatus.Payed;
        await _unitOfWork.SaveChangesAsync();

        return order;
    }

    public void ThrowIfNotOwenr(Order order, Guid customerId)
    {
        if (order.CustomerId != customerId)
        {
            throw new UnauthorizedAccessException();
        }
    }
}