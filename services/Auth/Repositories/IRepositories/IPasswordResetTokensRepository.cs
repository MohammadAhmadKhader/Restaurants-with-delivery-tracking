using Auth.Models;
using Shared.Data.Patterns.GenericRepository;

namespace Auth.Repositories.IRepositories;
public interface IPasswordResetTokensRepository : IGenericRepository<PasswordResetToken, Guid>
{
    
}