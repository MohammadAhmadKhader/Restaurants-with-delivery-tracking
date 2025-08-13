using FluentValidation;
using Restaurants.Contracts.Dtos.MenuItems;
using Restaurants.Utils;
using Shared.Validation.FluentValidation;

namespace Restaurants.Validation;
public class MenuItemCreateDtoValidator: AbstractValidator<MenuItemCreateDto>
{
    public static readonly List<string> defaultAllowedExtensions = [".jpg", ".jpeg", ".png"];
    public MenuItemCreateDtoValidator()
    {
        RuleFor(m => m.Name)
        .NotEmptyString()
        .Length(Constants.MinItemNameLength, Constants.MaxItemNameLength);

        RuleFor(m => m.Description)
        .NotEmptyString()
        .Length(Constants.MinItemDescriptionLength, Constants.MaxItemDescriptionLength);

        RuleFor(m => m.Price)
        .NotNull()
        .InclusiveBetween(Constants.MinItemPrice, Constants.MaxItemPrice);

        RuleFor(m => m.Image)
        .NotNull()
        .ValidImageFile();
    }
}