using Orders.Contracts.Dtos.Order;
using Orders.Models;

namespace Orders.Mappers;

public static class OrderMappers
{
    public static Order ToModel(this OrderPlaceDto dto)
    {
        return new()
        {
            DeliveryAddressId = dto.DeliveryAddressId,
            DeliveryTrackingEnabled = dto.DeliveryTrackingEnabled ?? true,
        };
    }

    public static OrderWithItemsCountViewDto ToViewWithItemsCountDto(this Order model)
    {
        return new()
        {
            Id = model.Id,
            CustomerId = model.CustomerId,
            RestaurantId = model.RestaurantId,
            DeliveryAddressId = model.DeliveryAddressId,
            DeliveryAgentId = model.DeliveryAgentId,
            Status = model.Status,
            TotalAmount = model.TotalAmount,
            PlacedAt = model.PlacedAt,
            DeliveredAt = model.DeliveredAt,
            CancelledAt = model.CancelledAt,
            DeliveryTrackingEnabled = model.DeliveryTrackingEnabled,
            ItemsCount = model.ItemsCount,
        };
    }

    public static OrderViewDto ToViewDto(this Order model)
    {
        return new()
        {
            Id = model.Id,
            CustomerId = model.CustomerId,
            RestaurantId = model.RestaurantId,
            DeliveryAddressId = model.DeliveryAddressId,
            DeliveryAgentId = model.DeliveryAgentId,
            Status = model.Status,
            TotalAmount = model.TotalAmount,
            PlacedAt = model.PlacedAt,
            DeliveredAt = model.DeliveredAt,
            CancelledAt = model.CancelledAt,
            DeliveryTrackingEnabled = model.DeliveryTrackingEnabled,
        };
    }

    public static OrderWithItemsViewDto ToViewWithItemsDto(this Order model)
    {
        return new()
        {
            Id = model.Id,
            CustomerId = model.CustomerId,
            RestaurantId = model.RestaurantId,
            DeliveryAddressId = model.DeliveryAddressId,
            DeliveryAgentId = model.DeliveryAgentId,
            Status = model.Status,
            TotalAmount = model.TotalAmount,
            PlacedAt = model.PlacedAt,
            DeliveredAt = model.DeliveredAt,
            CancelledAt = model.CancelledAt,
            DeliveryTrackingEnabled = model.DeliveryTrackingEnabled,
            Items = model.Items.Select(x => x.ToViewDto()).ToList(),
        };
    }
}