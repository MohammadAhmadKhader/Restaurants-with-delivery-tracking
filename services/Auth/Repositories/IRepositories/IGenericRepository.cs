namespace Auth.Repositories.IRepositories;
public interface IGenericRepository<TModel, TPrimaryKey> where TModel : class
{
    Task<TModel?> GetById(TPrimaryKey id);
    Task<TModel> Create(TModel model);
    Task<TModel?> Update(TPrimaryKey PK, Action<TModel> UpdateResource);
    Task<bool> Delete(TPrimaryKey id);
}