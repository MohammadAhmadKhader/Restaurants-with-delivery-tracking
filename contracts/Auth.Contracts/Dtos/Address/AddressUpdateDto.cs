using Shared.Contracts.Attributes;

namespace Auth.Contracts.Dtos.Address;

public class AddressUpdateDto(string? city, string? state, string? country, string? postalCode, string? addressLine)
{
    [Masked]
    public string? City { get; set; } = city?.Trim();
    public string? State { get; set; } = state?.Trim();
    public string? Country { get; set; } = country?.Trim();

    [Masked]
    public string? PostalCode { get; set; } = postalCode?.Trim();

    [Masked]
    public string? AddressLine { get; set; } = addressLine?.Trim();
}