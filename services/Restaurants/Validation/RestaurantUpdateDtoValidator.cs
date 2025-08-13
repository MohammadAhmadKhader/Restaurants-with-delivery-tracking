using FluentValidation;
using Restaurants.Contracts.Dtos.Restaurant;
using Restaurants.Utils;
using Shared.Validation.FluentValidation;

namespace Restaurants.Validation;
public class RestaurantUpdateDtoValidator: AbstractValidator<RestaurantUpdateDto>
{
    public RestaurantUpdateDtoValidator()
    {
        RuleFor(r => r.Name)
        .Length(Constants.MinRestaurantNameLength, Constants.MaxRestaurantNameLength);

        RuleFor(r => r.Description)
        .Length(Constants.MinRestaurantDescriptionLength, Constants.MaxRestaurantDescriptionLength);

        RuleFor(r => r.Phone)
        .ValidPhoneNumber();
    }
}