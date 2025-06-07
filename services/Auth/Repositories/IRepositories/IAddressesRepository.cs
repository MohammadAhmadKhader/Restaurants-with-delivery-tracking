using Auth.Models;

namespace Auth.Repositories.IRepositories;

public interface IAddressesRepository : IGenericRepository<Address, Guid>
{
    Task<List<Address>> FindAllByUserIdAsync(Guid Id, int page, int size);
    void Delete(Address address);
}