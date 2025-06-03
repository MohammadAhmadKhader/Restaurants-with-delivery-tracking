
using Auth.Data;
using Auth.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Auth.Repositories;

public class GenericRepository<TModel, TPrimaryKey>: IGenericRepository<TModel, TPrimaryKey> where TModel: class
{
    protected readonly AppDbContext _ctx;
    protected readonly DbSet<TModel> _dbSet;
    public GenericRepository(AppDbContext ctx)
    {
        _ctx = ctx;
        _dbSet = _ctx.Set<TModel>();
    }
    public virtual async Task<TModel> CreateAsync(TModel model)
    {
        var res = await _dbSet.AddAsync(model);
        await _ctx.SaveChangesAsync();
        return res.Entity;
    }

    public virtual async Task<bool> DeleteAsync(TPrimaryKey id)
    {
        var model = await _dbSet.FindAsync(id);
        if (model == null)
        {
            return false;
        }

        _dbSet.Remove(model);
        await _ctx.SaveChangesAsync();
        return true;
    }
    public virtual async Task<TModel?> FindByIdAsync(TPrimaryKey id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<TModel?> UpdateAsync(TPrimaryKey PK, Action<TModel> patcher)
    {
        var model = await _dbSet.FindAsync(PK);
        if (model == null)
        {
            return null;
        }

        patcher(model);
        await _ctx.SaveChangesAsync();
        return null;
    }
}