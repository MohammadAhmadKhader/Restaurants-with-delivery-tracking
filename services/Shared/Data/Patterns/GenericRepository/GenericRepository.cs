using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Shared.Data.Patterns.GenericRepository;

public class GenericRepository<TModel, TPrimaryKey> : IGenericRepository<TModel, TPrimaryKey> where TModel : class
{
    protected readonly DbContext _ctx;
    protected readonly DbSet<TModel> _dbSet;
    public GenericRepository(DbContext ctx)
    {
        _ctx = ctx;
        _dbSet = _ctx.Set<TModel>();
    }
    public async Task<(List<TModel> roles, int count)> FindAllOrderedDescAtAsync(
        int page,
        int size,
        string orderByPropertyName = "CreatedAt")
    {
        var skip = (page - 1) * size;
        var query = _dbSet.AsQueryable();

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(x => EF.Property<object>(x, orderByPropertyName))
            .Skip(skip)
            .Take(size)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<(List<TModel> roles, int count)> FindAllOrderedAscAtAsync(
        int page,
        int size,
        string orderByPropertyName = "CreatedAt")
    {
        var skip = (page - 1) * size;
        var query = _dbSet.AsQueryable();

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(x => EF.Property<object>(x, orderByPropertyName))
            .Skip(skip)
            .Take(size)
            .ToListAsync();

        return (items, totalCount);
    }
    public async Task<TModel?> FindByIdAsync(TPrimaryKey id)
    {
        return await _dbSet.FindAsync(id);
    }
    public async Task<bool> ExistsByIdAsync(TPrimaryKey id, string pkName = "Id")
    {
        return await _dbSet.AnyAsync(e => EF.Property<TPrimaryKey>(e, pkName)!.Equals(id));
    }
    public async Task<List<TModel>> FindManyByIdsAsync(IEnumerable<TPrimaryKey> ids, string pkName = "Id")
    {
        return await _dbSet
                .Where(x => ids.Contains(EF.Property<TPrimaryKey>(x, pkName)))
                .ToListAsync();
    }
    public async Task<TModel> CreateAsync(TModel model)
    {
        var res = await _dbSet.AddAsync(model);
        return res.Entity;
    }

    public async Task<TModel?> UpdateAsync(TPrimaryKey PK, Action<TModel> patcher)
    {
        var model = await _dbSet.FindAsync(PK);
        if (model == null)
        {
            return null;
        }

        patcher(model);
        return null;
    }

    public async Task<bool> DeleteAsync(TPrimaryKey id)
    {
        var model = await _dbSet.FindAsync(id);
        if (model == null)
        {
            return false;
        }

        _dbSet.Remove(model);
        return true;
    }

    public EntityEntry<TModel> Delete(TModel model)
    {
        return _dbSet.Remove(model);
    }

    public async Task<int> DeleteManyAsync(IEnumerable<TPrimaryKey> ids, string pkName = "Id")
    {
        var models = await _dbSet
                .Where(x => ids.Contains(EF.Property<TPrimaryKey>(x, pkName)))
                .ToListAsync();
        if (!models.Any())
        {
            return 0;
        }

        _dbSet.RemoveRange(models);
        return models.Count;
    }

    public async Task<TModel?> FindByMatchAsync(Expression<Func<TModel, bool>> match)
    {
        return await _dbSet.FirstOrDefaultAsync(match);
    }
    
    public async Task<bool> ExistsByMatchAsync(Expression<Func<TModel, bool>> match)
    {
        return await _dbSet.AnyAsync(match);
    }
}