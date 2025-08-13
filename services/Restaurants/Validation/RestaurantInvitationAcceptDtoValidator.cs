using FluentValidation;
using Restaurants.Contracts.Dtos.Restaurant;
using Restaurants.Utils;

namespace Restaurants.Validation;
public class RestaurantInvitationAcceptDtoValidator: AbstractValidator<RestaurantInvitationAcceptDto>
{
    public RestaurantInvitationAcceptDtoValidator()
    {
        RuleFor(dto => dto.User.FirstName)
            .NotEmpty();

        RuleFor(dto => dto.User.LastName)
            .NotEmpty();

        RuleFor(dto => dto.User.Password)
            .NotEmpty()
            .MinimumLength(6);

        RuleFor(dto => dto.User.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(Constants.MaxEmailLength);

        RuleFor(dto => dto.InvitationId)
          .NotEmpty();
    }
}