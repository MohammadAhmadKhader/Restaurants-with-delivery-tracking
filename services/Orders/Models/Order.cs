using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Orders.Contracts.Enums;
using Orders.Utils;
namespace Orders.Models;

public class Order: ITenantModel, ICustomerModel
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
    public DateTime? CancelledAt { get; set; }
    public bool DeliveryTrackingEnabled { get; set; }

    [NotMapped]
    public int ItemsCount { get; set; }
    public ICollection<OrderItem> Items { get; set; } = new HashSet<OrderItem>();
}