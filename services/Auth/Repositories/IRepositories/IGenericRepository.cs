namespace Auth.Repositories.IRepositories;
public interface IGenericRepository<TModel, TPrimaryKey> where TModel : class
{
    Task<TModel?> FindById(TPrimaryKey id);
    Task<TModel> Create(TModel model);
    Task<TModel?> Update(TPrimaryKey PK, Action<TModel> UpdateResource);
    Task<bool> Delete(TPrimaryKey id);
}