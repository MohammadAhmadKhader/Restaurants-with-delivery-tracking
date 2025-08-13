using Auth.Data;
using Auth.Models;
using Auth.Repositories.IRepositories;
using Shared.Data.Patterns.GenericRepository;

namespace Auth.Repositories;

public class PasswordResetTokensRepository(AppDbContext ctx) : 
    GenericRepository<PasswordResetToken, Guid, AppDbContext>(ctx), IPasswordResetTokensRepository
{
    
}