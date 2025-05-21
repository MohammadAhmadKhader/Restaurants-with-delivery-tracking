using System.ComponentModel.DataAnnotations;

public class CurrentLocation
{
    [Key]
    public Guid DeliveryAgentId { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}