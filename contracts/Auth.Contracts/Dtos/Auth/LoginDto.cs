namespace Auth.Contracts.Dtos.Auth;


public class LoginDto(string email, string password)
{
    public string Email { get; init; } = email?.Trim()!;
    public string Password { get; init; } = password?.Trim()!;
}