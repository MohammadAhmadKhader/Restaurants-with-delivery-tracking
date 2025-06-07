using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Auth.Utils;

namespace Auth.Models;
public class Address
{
    [Key]
    public Guid Id { get; set; } 
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(Constants.MaxCityLength)]
    public string City { get; set; } = default!;

    [MaxLength(Constants.MaxStateLength)]
    public string? State { get; set; } = default!;

    [Required]
    [MaxLength(Constants.MaxCountryLength)]
    public string Country { get; set; } = default!;

    [MaxLength(Constants.MaxPostalCodeLength)]
    public string? PostalCode { get; set; }

    [MaxLength(Constants.MaxAddressLineLength)]
    public string? AddressLine { get; set; }

    [Required]
    public decimal Latitude { get; set; }

    [Required]
    public decimal Longitude { get; set; }

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }
}