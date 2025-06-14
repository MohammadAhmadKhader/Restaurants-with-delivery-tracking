using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Shared.Data.Patterns.GenericRepository;

public interface IGenericRepository<TModel, TPrimaryKey> where TModel : class
{
    Task<(List<TModel> roles, int count)> FindAllOrderedDescAtAsync(int page, int size, string orderByPropertyName = "CreatedAt");
    Task<(List<TModel> roles, int count)> FindAllOrderedAscAtAsync(int page, int size, string orderByPropertyName = "CreatedAt");
    Task<TModel?> FindByIdAsync(TPrimaryKey id);
    Task<List<TModel>> FindManyByIdsAsync(IEnumerable<TPrimaryKey> ids, string pkName = "Id");
    Task<bool> ExistsByIdAsync(TPrimaryKey id, string pkName = "Id");
    Task<TModel> CreateAsync(TModel model);
    Task<TModel?> UpdateAsync(TPrimaryKey PK, Action<TModel> UpdateResource);
    Task<bool> DeleteAsync(TPrimaryKey id);
    Task<int> DeleteManyAsync(IEnumerable<TPrimaryKey> ids, string pkName = "Id");
    EntityEntry<TModel> Delete(TModel model);
}