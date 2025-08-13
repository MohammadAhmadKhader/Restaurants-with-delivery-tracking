using FluentValidation;
using Restaurants.Contracts.Dtos.Menu;
using Shared.Validation.FluentValidation;

namespace Restaurants.Validation;
class MenuAddItemsDtoValidator: AbstractValidator<MenuAddItemsDto>
{
    public MenuAddItemsDtoValidator()
    {
        RuleFor(x => x.MenuItemsIds)
        .MustHaveUniqueValues(itemId => itemId);
    }
}