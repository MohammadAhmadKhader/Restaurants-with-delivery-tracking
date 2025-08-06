using Orders.Contracts.Dtos.OrderItem;
using Orders.Models;

namespace Orders.Mappers;
public static class OrderItemMappers
{
    public static OrderItem ToModel(this OrderItemCreateDto dto)
    {
        return new()
        {
            MenuItemId = dto.MenuItemId,
            Quantity = dto.Quantity,
            Price = dto.Price
        };
    }

    public static OrderItemViewDto ToViewDto(this OrderItem model)
    {
        return new()
        {
            Id = model.Id,
            MenuItemId = model.MenuItemId,
            OrderId = model.OrderId,
            Price = model.Price,
            Quantity = model.Quantity
        };
    }
}