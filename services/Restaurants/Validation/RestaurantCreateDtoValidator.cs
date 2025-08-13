using FluentValidation;
using Restaurants.Contracts.Dtos.Restaurant;
using Restaurants.Utils;
using Shared.Validation.FluentValidation;

namespace Restaurants.Validation;
public class RestaurantCreateDtoValidator: AbstractValidator<RestaurantCreateDto>
{
    public RestaurantCreateDtoValidator()
    {
        RuleFor(r => r.Name)
        .NotEmptyString()
        .Length(Constants.MinRestaurantNameLength, Constants.MaxRestaurantNameLength);

        RuleFor(r => r.Description)
        .NotEmptyString()
        .Length(Constants.MinRestaurantDescriptionLength, Constants.MaxRestaurantDescriptionLength);

        RuleFor(r => r.Phone)
        .NotEmptyString()
        .ValidPhoneNumber();
    }
}