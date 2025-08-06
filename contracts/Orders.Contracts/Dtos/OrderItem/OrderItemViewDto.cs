namespace Orders.Contracts.Dtos.OrderItem;
public class OrderItemViewDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public int MenuItemId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}