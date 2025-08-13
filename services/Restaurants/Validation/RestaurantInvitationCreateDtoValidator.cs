using FluentValidation;
using Restaurants.Contracts.Dtos.Restaurant;
using Restaurants.Utils;
using Shared.Validation.FluentValidation;

namespace Restaurants.Validation;
public class RestaurantInvitationCreateDtoValidator: AbstractValidator<RestaurantInvitationCreateDto>
{
    public RestaurantInvitationCreateDtoValidator()
    {
        RuleFor(x => x.Email)
          .NotEmptyString()
          .EmailAddress()
          .MaximumLength(Constants.MaxEmailLength);
    }
}