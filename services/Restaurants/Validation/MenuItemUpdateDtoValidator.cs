using FluentValidation;
using Restaurants.Contracts.Dtos.MenuItems;
using Restaurants.Models;
using Restaurants.Utils;
using Shared.Validation.FluentValidation;

namespace Restaurants.Validation;
public class MenuItemUpdateDtoValidator: AbstractValidator<MenuItemUpdateDto>
{
    public static string AtLeastOneRequiredErrorMessage { get; }
    static MenuItemUpdateDtoValidator()
    {
        var atLeastOneRequiredErrMsg = ValidationMessagesBuilder.AtLeastOneRequired(
            nameof(MenuItem.Name),
            nameof(MenuItem.Description),
            nameof(MenuItem.Price),
            nameof(MenuItem.IsAvailable),
            nameof(MenuItemUpdateDto.Image)
        );

        AtLeastOneRequiredErrorMessage = atLeastOneRequiredErrMsg;
    }

    public MenuItemUpdateDtoValidator()
    {
        RuleFor(m => m.Name)
        .Length(Constants.MinItemNameLength, Constants.MaxItemNameLength);

        RuleFor(m => m.Description)
        .Length(Constants.MinItemDescriptionLength, Constants.MaxItemDescriptionLength);

        RuleFor(m => m.Price)
        .InclusiveBetween(Constants.MinItemPrice, Constants.MaxItemPrice);

        RuleFor(m => m.Image)
        .ValidImageFile();

        RuleFor(a => a)
        .Must(a => a.Name != null || a.Description != null || a.Price != null || a.IsAvailable != null || a.Image != null)
        .WithName(nameof(MenuItem))
        .WithMessage(AtLeastOneRequiredErrorMessage);
    }
}