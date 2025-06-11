namespace Auth.Dtos.Address;

public class AddressCreateDto
{
    public string City { get; set; } = default!;
    public string? State { get; set; } = default!;
    public string Country { get; set; } = default!;
    public string? PostalCode { get; set; }
    public string? AddressLine { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
}