namespace Orders.Contracts.Dtos.Order;

public class OrderPlaceDto
{
    public bool? DeliveryTrackingEnabled { get; set; }
    public Guid DeliveryAddressId { get; set; }
    public List<Item> Items { get; set; } = [];
}

public class Item
{
    public int Quantity { get; set; }
    public int ItemId { get; set; }
}