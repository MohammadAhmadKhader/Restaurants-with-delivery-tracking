namespace Auth.Dtos.Role;

public class RoleCreateDto(string? name, string? displayName)
{
    public string Name { get; set; } = name?.Trim()!;
    public string DisplayName { get; set; } = displayName?.Trim()!;
}