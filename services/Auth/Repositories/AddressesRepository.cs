using Auth.Data;
using Auth.Models;
using Auth.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Auth.Repositories;

public class AddressesRepository(AppDbContext ctx) : GenericRepository<Address, Guid>(ctx), IAddressesRepository
{
    public async Task<List<Address>> FindAllByUserIdAsync(Guid Id, int page, int size)
    {
        var skip = (page - 1) * size;
        return await _dbSet.Where(a => a.UserId == Id)
        .Skip(skip)
        .Take(size)
        .ToListAsync();
    }

    public void Delete(Address address)
    {
        _dbSet.Remove(address);
    }
}