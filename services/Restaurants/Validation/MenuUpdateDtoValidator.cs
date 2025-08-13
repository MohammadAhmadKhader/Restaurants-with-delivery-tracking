using FluentValidation;
using Restaurants.Contracts.Dtos.Menu;
using Restaurants.Models;
using Restaurants.Utils;
using Shared.Validation.FluentValidation;

namespace Restaurants.Validation;
public class MenuUpdateDtoValidator: AbstractValidator<MenuUpdateDto>
{
    public static string AtLeastOneRequiredErrorMessage { get; }
    static MenuUpdateDtoValidator()
    {
        var atLeastOneRequiredErrMsg = ValidationMessagesBuilder.AtLeastOneRequired(
            nameof(Menu.Name),
            nameof(Menu.Category));

        AtLeastOneRequiredErrorMessage = atLeastOneRequiredErrMsg;
    }
    
    public MenuUpdateDtoValidator()
    {
        RuleFor(m => m.Name)
        .Length(Constants.MinMenuNameLength, Constants.MaxMenuNameLength);

        RuleFor(m => m.Category)
        .Length(Constants.MinMenuCategoryLength, Constants.MaxMenuCategoryLength);

        RuleFor(a => a)
        .Must(a => a.Name != null || a.Category != null)
        .WithName(nameof(Menu))
        .WithMessage(AtLeastOneRequiredErrorMessage);
    }
}