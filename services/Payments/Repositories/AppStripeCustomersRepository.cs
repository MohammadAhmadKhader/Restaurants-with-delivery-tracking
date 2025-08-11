using Microsoft.EntityFrameworkCore;
using Payments.Data;
using Payments.Models;
using Payments.Repositories.IRepositories;
using Shared.Data.Patterns.GenericRepository;

namespace Payments.Repositories;

public class AppStripeCustomersRepository(AppDbContext ctx) : GenericRepository<AppStripeCustomer, Guid, AppDbContext>(ctx), IAppStripeCustomersRepository
{
    public async Task<AppStripeCustomer?> FindByUserIdAsync(Guid userId)
    {
        return await _dbSet.Where(s => s.UserId == userId).FirstOrDefaultAsync();
    }

    public async Task<AppStripeCustomer?> FindByStripeCustomerIdAsync(string stripeCustomerId)
    {
        return await _dbSet.Where(s => s.StripeCustomerId == stripeCustomerId).FirstOrDefaultAsync();
    }
}