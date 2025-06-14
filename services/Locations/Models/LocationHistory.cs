using System.ComponentModel.DataAnnotations;

namespace Locations.Models;
public class LocationHistory
{
    [Key]
    public Guid Id { get; set; }
    public Guid DeliveryAgentId { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}