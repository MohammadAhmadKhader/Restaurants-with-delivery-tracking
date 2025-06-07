namespace Auth.Dtos.Address;

public class AddressViewDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string City { get; set; } = default!;
    public string? State { get; set; }
    public string Country { get; set; } = default!;
    public string? PostalCode { get; set; }
    public string? AddressLine { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
}