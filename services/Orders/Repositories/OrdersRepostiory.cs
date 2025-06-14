using Orders.Data;
using Orders.Repositories.IRepositories;

namespace Orders.Repositories;
public class OrdersRepository(AppDbContext ctx): IOrdersRepository
{

}