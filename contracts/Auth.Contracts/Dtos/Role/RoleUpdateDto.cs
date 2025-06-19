namespace Auth.Contracts.Dtos.Role;

public class RoleUpdateDto(string? name, string? displayName)
{
    public string? Name { get; set; } = name?.Trim()!;
    public string? DisplayName { get; set; } = displayName?.Trim()!;
}