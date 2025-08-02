using Restaurants.Contracts.Dtos.Menu;
using Restaurants.Models;

namespace Restaurants.Mappers;

public static class MenuMappers
{
    public static Menu ToModel(this MenuCreateDto dto)
    {
        return new Menu
        {
            Name = dto.Name,
            Category = dto.Category,
            NormalizedName = dto.Name.ToUpper(),
        };
    }

    public static MenuViewDto ToViewDto(this Menu menu)
    {
        return new MenuViewDto
        {
            Id = menu.Id,
            Name = menu.Name,
            Category = menu.Category,
            RestaurantId = menu.RestaurantId,
        };
    }
    public static MenuWithItemsViewDto ToViewWithItemsDto(this Menu menu)
    {
        return new MenuWithItemsViewDto
        {
            Id = menu.Id,
            Name = menu.Name,
            Category = menu.Category,
            RestaurantId = menu.RestaurantId,
            Items = menu.Items.Select(x => x.ToViewDto()).ToList()
        };
    }

    public static void PatchModel(this MenuUpdateDto dto, Menu model)
    {
        if (!string.IsNullOrWhiteSpace(dto.Category))
        {
            model.Category = dto.Category;
        }

        if (!string.IsNullOrWhiteSpace(dto.Name))
        {
            model.Name = dto.Name;
            model.NormalizedName = dto.Name.ToUpper();
        }
    }
}