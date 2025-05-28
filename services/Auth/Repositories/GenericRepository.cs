
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
    public virtual async Task<TModel> Create(TModel model)
    {
        var res = await _dbSet.AddAsync(model);
        return res.Entity;
    }

    public virtual async Task<bool> Delete(TPrimaryKey id)
    {
        var model = await _dbSet.FindAsync(id);
        if (model == null)
        {
            return false;
        }

        _dbSet.Remove(model);
        return true;
    }
    public virtual async Task<TModel?> FindById(TPrimaryKey id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<TModel?> Update(TPrimaryKey PK, Action<TModel> patcher)
    {
        var model = await _dbSet.FindAsync(PK);
        if (model == null)
        {
            return null;
        }

        patcher(model);
        return null;
    }
}