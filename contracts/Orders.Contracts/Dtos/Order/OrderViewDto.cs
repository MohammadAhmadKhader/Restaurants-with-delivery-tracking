using System.Text.Json.Serialization;
using Orders.Contracts.Enums;

namespace Orders.Contracts.Dtos.Order;
public class OrderViewDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid RestaurantId { get; set; }
    public Guid DeliveryAddressId { get; set; }
    public Guid? DeliveryAgentId { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OrderStatus? Status { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime PlacedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeliveredAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public bool DeliveryTrackingEnabled { get; set; } = true;
}