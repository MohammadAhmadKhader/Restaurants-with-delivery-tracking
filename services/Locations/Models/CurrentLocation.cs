using System.ComponentModel.DataAnnotations;

namespace Locations.Models;

public class CurrentLocation
{
    [Key]
    public Guid DeliveryAgentId { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}