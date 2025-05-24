using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auth.Models;
public class Address
{
    [Key]
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public required string City { get; set; }
    public required string State { get; set; }
    public required string Country { get; set; }
    public string? PostalCode { get; set; }
    public string? AddressLine { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }
}