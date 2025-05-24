using Auth.Data;
using Auth.Models;
using Auth.Repositories;
using Auth.Repositories.IRepositories;

namespace Auth.Repositories;
public class AddressesRepository(AppDbContext ctx) : GenericRepository<Address, Guid>(ctx), IAddressesRepository
{

}