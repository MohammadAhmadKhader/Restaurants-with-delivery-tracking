using Restaurants.Contracts.Dtos.MenuItems;
using Restaurants.Models;

namespace Restaurants.Mappers;

public static class MenuItemMappers
{
    public static MenuItemViewDto ToViewDto(this MenuItem item)
    {
        return new MenuItemViewDto
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            Price = item.Price,
            IsAvailable = item.IsAvailable,
            ImageUrl = item.ImageUrl,
            ImagePublicId = item.ImagePublicId
        };
    }

    public static MenuItem ToModel(this MenuItemCreateDto dto)
    {
        var item = new MenuItem
        {
            Name = dto.Name,
            NormalizedName = dto.Name.ToUpper(),
            Description = dto.Description,
            Price = dto.Price,
        };

        if (dto.IsAvailable.HasValue)
        {
            item.IsAvailable = dto.IsAvailable.Value;
        }

        return item;
    }

    public static void PatchModel(this MenuItemUpdateDto dto, MenuItem model)
    {
        if (!string.IsNullOrWhiteSpace(dto.Name))
        {
            model.Name = dto.Name;
            model.NormalizedName = dto.Name.ToUpper();
        }

        if (!string.IsNullOrWhiteSpace(dto.Description))
        {
            model.Description = dto.Description;
        }

        if (dto.Price.HasValue)
        {
            model.Price = dto.Price.Value;
        }

        if (dto.IsAvailable.HasValue)
        {
            model.IsAvailable = dto.IsAvailable.Value;
        }
    }
}