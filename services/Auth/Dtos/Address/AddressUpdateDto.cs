namespace Auth.Dtos.Address;

public class AddressUpdateDto
{
    public string? City { get; set; } = default!;
    public string? State { get; set; } = default!;
    public string? Country { get; set; } = default!;
    public string? PostalCode { get; set; }
    public string? AddressLine { get; set; }
}