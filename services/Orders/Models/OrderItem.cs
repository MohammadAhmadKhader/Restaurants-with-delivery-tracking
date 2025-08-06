using System.ComponentModel.DataAnnotations;

namespace Orders.Models;
public class OrderItem
{
    [Key]
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public int MenuItemId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public Order? Order { get; set; }
}
