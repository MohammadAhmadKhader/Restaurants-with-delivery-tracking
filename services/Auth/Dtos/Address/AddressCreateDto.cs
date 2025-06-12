namespace Auth.Dtos.Address;

public class AddressCreateDto(string city, string? state, string country, string? postalCode, string? addressLine)
{
    public string City { get; set; } = city?.Trim()!;
    public string? State { get; set; } = state?.Trim();
    public string Country { get; set; } = country?.Trim()!;
    public string? PostalCode { get; set; } = postalCode?.Trim();
    public string? AddressLine { get; set; } = addressLine?.Trim();
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
}