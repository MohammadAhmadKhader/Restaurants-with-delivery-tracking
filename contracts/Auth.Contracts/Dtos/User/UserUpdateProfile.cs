using Shared.Contracts.Attributes;

namespace Auth.Contracts.Dtos.User;

public class UserUpdateProfile(string firstName, string lastName)
{
    [Masked]
    public string? FirstName { get; init; } = firstName?.Trim();

    [Masked]
    public string? LastName { get; init; } = lastName?.Trim();
}