using Payments.Models;
using Shared.Data.Patterns.GenericRepository;

namespace Payments.Repositories.IRepositories;

public interface IAppStripeCustomersRepository : IGenericRepository<AppStripeCustomer, Guid>
{
    Task<AppStripeCustomer?> FindByUserIdAsync(Guid userId);

    Task<AppStripeCustomer?> FindByStripeCustomerIdAsync(string stripeCustomerId);
}