using Shared.Contracts.Attributes;

namespace Auth.Contracts.Dtos.Address;

public class AddressCreateDto(string city, string? state, string country, string? postalCode, string? addressLine)
{
    [Masked]
    public string City { get; set; } = city?.Trim()!;

    [Masked]
    public string? State { get; set; } = state?.Trim();

    public string Country { get; set; } = country?.Trim()!;

    [Masked]
    public string? PostalCode { get; set; } = postalCode?.Trim();

    [Masked]
    public string? AddressLine { get; set; } = addressLine?.Trim();

    [Masked]
    public decimal? Latitude { get; set; }

    [Masked]
    public decimal? Longitude { get; set; }
}