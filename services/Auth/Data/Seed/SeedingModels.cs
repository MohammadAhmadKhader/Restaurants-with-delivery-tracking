namespace Auth.Data.Seed;
public record SeedDataModel
{
    public List<SeedUser> Users { get; set; } = default!;
    public List<SeedRole> Roles { get; set; } = default!;
    public List<SeedPermission> Permissions { get; set; } = default!;
}

public record SeedUser
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public bool EmailConfirmed { get; set; } = default!;
    public string Password { get; set; } = default!;
}

public record SeedRole
{
    public string Name { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
}

public record SeedPermission
{
    public string Name { get; set; } = default!;
}