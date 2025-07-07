namespace Auth.Contracts.Dtos.RestaurantRole;
public class RestaurantRoleUpdateDto(string? name, string? displayName)
{
    public string? Name { get; set; } = name?.Trim()!;
    public string? DisplayName { get; set; } = displayName?.Trim()!;
}