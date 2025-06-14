using Auth.Models;
using Shared.Data.Patterns.GenericRepository;

namespace Auth.Repositories.IRepositories;

public interface IAddressesRepository : IGenericRepository<Address, Guid>
{
    Task<(List<Address> addresses, int count)> FindAllByUserIdAsync(Guid Id, int page, int size);
}