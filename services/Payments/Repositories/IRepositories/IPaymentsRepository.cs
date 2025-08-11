using Payments.Models;
using Shared.Data.Patterns.GenericRepository;

namespace Payments.Repositories.IRepositories;
public interface IPaymentsRepository: IGenericRepository<Payment, Guid>
{

}