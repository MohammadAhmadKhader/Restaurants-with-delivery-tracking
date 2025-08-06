using Microsoft.EntityFrameworkCore;
using Orders.Data;
using Orders.Models;
using Orders.Repositories.IRepositories;
using Shared.Data.Patterns.GenericRepository;
namespace Orders.Repositories;

public class OrdersRepository(AppDbContext ctx) : GenericRepository<Order, Guid, AppDbContext>(ctx), IOrdersRepository
{
    public async Task<(List<Order> orders, int count)> FindAllForCustomerWithItemsAsync(int page, int size, Guid customerId)
    {
        var skip = (page - 1) * size;
        var orders = await _dbSet
            .Where(o => o.CustomerId == customerId)
            .Select(o => new
            {
                Order = o,
                ItemCount = _ctx.OrderItems.Count(oi => oi.OrderId == o.Id)
            })
            .OrderByDescending(x => x.Order.PlacedAt)
            .Skip(skip)
            .Take(size)
            .Select(x => new Order
            {
                Id = x.Order.Id,
                CustomerId = x.Order.CustomerId,
                RestaurantId = x.Order.RestaurantId,
                DeliveryAddressId = x.Order.DeliveryAddressId,
                DeliveryAgentId = x.Order.DeliveryAgentId,
                Status = x.Order.Status,
                TotalAmount = x.Order.TotalAmount,
                PlacedAt = x.Order.PlacedAt,
                DeliveredAt = x.Order.DeliveredAt,
                CancelledAt = x.Order.CancelledAt,
                DeliveryTrackingEnabled = x.Order.DeliveryTrackingEnabled,
                ItemsCount = x.ItemCount
            })
            .ToListAsync();

        var count = await _dbSet.Where(o => o.CustomerId == customerId).CountAsync();
        return (orders, count);
    }

    public async Task<Order?> FindByIdWithItemsAsync(Guid id)
    {
        return await _dbSet
                .Where(o => o.Id == id)
                .Include(o => o.Items)
                .FirstOrDefaultAsync();
    }

    public async Task<Order?> FindByIdForCustomerWithItemsAsync(Guid id, Guid customerId)
    {
        return await _dbSet
                .Where(o => o.Id == id && o.CustomerId == customerId)
                .Include(o => o.Items)
                .FirstOrDefaultAsync();
    }
}