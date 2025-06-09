using Auth.Dtos.Auth;
using Auth.Utils;
using FluentValidation;
using Shared.Extensions;

namespace Auth.Extensions.FluentValidationValidators;

class RegisterDtoValidator: AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmptyString()
            .Length(Constants.MinFirstNameLength, Constants.MaxFirstNameLength);

        RuleFor(x => x.LastName)
            .NotEmptyString()
            .Length(Constants.MinLastNameLength, Constants.MaxLastNameLength);

        RuleFor(x => x.Email)
            .NotEmptyString()
            .EmailAddress()
            .MaximumLength(Constants.MaxEmailLength);

        RuleFor(x => x.Password)
            .NotEmptyString()
            .Length(Constants.MinPasswordLength, Constants.MaxPasswordLength);
    }
}