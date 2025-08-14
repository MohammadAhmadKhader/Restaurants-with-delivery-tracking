using Shared.Contracts.Attributes;

namespace Auth.Contracts.Dtos.Address;

public class AddressViewDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    [Masked]
    public string City { get; set; } = default!;

    [Masked]
    public string? State { get; set; }
    public string Country { get; set; } = default!;

    [Masked]
    public string? PostalCode { get; set; }

    [Masked]
    public string? AddressLine { get; set; }

    [Masked]
    public decimal Latitude { get; set; }

    [Masked]
    public decimal Longitude { get; set; }
}