namespace Auth.Dtos.Auth;

public class RegisterDto(string firstName, string lastName, string email, string password)
{
    public string FirstName { get; set; } = firstName;
    public string LastName { get; set; } = lastName;
    public string Email { get; init; } = email?.ToLower() ?? string.Empty;
    public string Password { get; set; } = password;
}