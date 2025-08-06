using Orders.Models;
using Shared.Data.Patterns.GenericRepository;

namespace Orders.Repositories.IRepositories;

public interface IOrdersRepository : IGenericRepository<Order, Guid>
{
    Task<Order?> FindByIdWithItemsAsync(Guid id);
    Task<Order?> FindByIdForCustomerWithItemsAsync(Guid id, Guid customerId);
    Task<(List<Order> orders, int count)> FindAllForCustomerWithItemsAsync(int page, int size, Guid customerId);
}