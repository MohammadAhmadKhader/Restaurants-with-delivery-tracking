namespace Auth.Dtos.Auth;

public class LoginDto(string email, string password)
{
    public string Email { get; init; } = email.ToLower();
    public string Password { get; init; } = password;
}