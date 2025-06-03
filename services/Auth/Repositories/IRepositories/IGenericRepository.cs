namespace Auth.Repositories.IRepositories;
public interface IGenericRepository<TModel, TPrimaryKey> where TModel : class
{
    Task<TModel?> FindByIdAsync(TPrimaryKey id);
    Task<TModel> CreateAsync(TModel model);
    Task<TModel?> UpdateAsync(TPrimaryKey PK, Action<TModel> UpdateResource);
    Task<bool> DeleteAsync(TPrimaryKey id);
}