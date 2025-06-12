namespace Auth.Dtos.Auth;

public class RegisterDto(string firstName, string lastName, string email, string password)
{
    public string FirstName { get; set; } = firstName?.Trim()!;
    public string LastName { get; set; } = lastName?.Trim()!;
    public string Email { get; init; } = email?.ToLower().Trim()!;
    public string Password { get; set; } = password?.Trim()!;
}