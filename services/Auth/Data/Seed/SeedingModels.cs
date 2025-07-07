namespace Auth.Data.Seed;
public record SeedDataModel
{
    public List<SeedUser> Users { get; set; } = default!;
    public List<SeedAddress> Addresses { get; set; } = default!;
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
    public Guid? RestaurantId { get; set; }
    public bool IsGlobal { get; set; }
}

public record SeedRole
{
    public string Name { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
}

public record SeedPermission
{
    public string Name { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public bool IsDefaultUser { get; set; }
    public bool IsDefaultAdmin { get; set; }
    public bool IsDefaultSuperAdmin { get; set; }
}

public record SeedAddress
{
    public string City { get; set; } = default!;
    public string State { get; set; } = default!;
    public string Country { get; set; } = default!;
    public string PostalCode { get; set; } = default!;
    public string AddressLine { get; set; } = default!;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
}