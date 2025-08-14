using Shared.Contracts.Attributes;

namespace Auth.Contracts.Dtos.Auth;


public class LoginDto(string email, string password)
{
    [Masked]
    public string Email { get; init; } = email?.Trim()!;

    [Masked]
    public string Password { get; init; } = password?.Trim()!;
}