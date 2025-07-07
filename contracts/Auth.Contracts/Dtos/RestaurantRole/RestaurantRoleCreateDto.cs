namespace Auth.Contracts.Dtos.RestaurantRole;
public class RestaurantRoleCreateDto(string? name, string? displayName)
{
    public string Name { get; set; } = name?.Trim()!;
    public string DisplayName { get; set; } = displayName?.Trim()!;
}