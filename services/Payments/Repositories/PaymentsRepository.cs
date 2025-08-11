using Payments.Data;
using Payments.Models;
using Payments.Repositories.IRepositories;
using Shared.Data.Patterns.GenericRepository;

namespace Payments.Repositories;

public class PaymentsRepository(AppDbContext ctx): GenericRepository<Payment, Guid, AppDbContext>(ctx), IPaymentsRepository
{
    
}