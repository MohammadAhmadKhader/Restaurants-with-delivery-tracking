namespace Auth.Dtos.User;

public class UserUpdateProfile(string firstName, string lastName)
{
    public string? FirstName { get; init; } = firstName?.Trim();
    public string? LastName { get; init; } = lastName?.Trim();
}