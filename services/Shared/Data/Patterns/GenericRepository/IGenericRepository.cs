using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Shared.Data.Patterns.GenericRepository;

public interface IGenericRepository<TModel, TPrimaryKey> where TModel : class
{
    /// <summary>
    /// this meant only for seeding purposes or similar cases, not for using in normal situations
    /// </summary>
    Task<List<TModel>> FindAllAsync();
    Task<(List<TModel> roles, int count)> FindAllOrderedDescAtAsync(int page, int size, string orderByPropertyName = "CreatedAt");
    Task<(List<TModel> roles, int count)> FindAllOrderedAscAtAsync(int page, int size, string orderByPropertyName = "CreatedAt");
    Task<TModel?> FindByIdAsync(TPrimaryKey id);
    Task<TModel?> FindByMatchAsync(Expression<Func<TModel, bool>> match);
    Task<List<TModel>> FindManyByIdsAsync(IEnumerable<TPrimaryKey> ids, string pkName = "Id");
    Task<bool> ExistsByIdAsync(TPrimaryKey id, string pkName = "Id");
    Task<bool> ExistsByMatchAsync(Expression<Func<TModel, bool>> match);
    Task<TModel> CreateAsync(TModel model);
    Task<TModel?> UpdateAsync(TPrimaryKey PK, Action<TModel> UpdateResource);
    Task<bool> DeleteAsync(TPrimaryKey id);
    Task<int> DeleteManyAsync(IEnumerable<TPrimaryKey> ids, string pkName = "Id");
    EntityEntry<TModel> Delete(TModel model);
}