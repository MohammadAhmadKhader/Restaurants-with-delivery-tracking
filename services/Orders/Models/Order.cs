using System.ComponentModel.DataAnnotations;
namespace Orders.Models;
public enum OrderStatus { PLACED, ACCEPTED, PREPARING, OUT_FOR_DELIVERY, DELIVERED, CANCELLED }
public class Order
{
    [Key]
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid RestaurantId { get; set; }
    public Guid DeliveryAddressId { get; set; }
    public Guid? DeliveryAgentId { get; set; } // ? maybe removed?
    public OrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime PlacedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeliveredAt { get; set; }
    public bool DeliveryTrackingEnabled { get; set; } = true;
    public ICollection<OrderItem> Items { get; set; } = new HashSet<OrderItem>();
}