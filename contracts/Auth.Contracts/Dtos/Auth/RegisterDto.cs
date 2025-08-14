using Shared.Contracts.Attributes;

namespace Auth.Contracts.Dtos.Auth;

public class RegisterDto(string firstName, string lastName, string email,string password)
{
    [Masked]
    public string FirstName { get; set; } = firstName?.Trim()!;

    [Masked]
    public string LastName { get; set; } = lastName?.Trim()!;

    [Masked]
    public string Email { get; init; } = email?.ToLower().Trim()!;

    [Masked]
    public string Password { get; set; } = password?.Trim()!;
}