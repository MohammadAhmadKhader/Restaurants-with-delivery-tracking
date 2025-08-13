using FluentValidation;
using Restaurants.Contracts.Dtos.Menu;
using Restaurants.Utils;
using Shared.Validation.FluentValidation;

namespace Restaurants.Validation;
class MenuCreateDtoValidator: AbstractValidator<MenuCreateDto>
{
    public MenuCreateDtoValidator()
    {
        RuleFor(m => m.Name)
        .NotEmptyString()
        .Length(Constants.MinMenuNameLength, Constants.MaxMenuNameLength);

        RuleFor(m => m.Category)
        .NotEmptyString()
        .Length(Constants.MinMenuCategoryLength, Constants.MaxMenuCategoryLength);
    }
}