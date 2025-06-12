using Auth.Data;
using Auth.Models;
using Auth.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Auth.Repositories;

public class AddressesRepository(AppDbContext ctx) : GenericRepository<Address, Guid>(ctx), IAddressesRepository
{
    public async Task<(List<Address> addresses, int count)> FindAllByUserIdAsync(Guid Id, int page, int size)
    {
        var skip = (page - 1) * size;
        var addresses = await _dbSet.Where(a => a.UserId == Id)
        .Skip(skip)
        .Take(size)
        .OrderBy(x => x.Id)
        .ToListAsync();

        var count = await _dbSet.Where(a => a.UserId == Id).CountAsync();

        return (addresses, count);
    }

    public void Delete(Address address)
    {
        _dbSet.Remove(address);
    }
}