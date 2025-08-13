using Auth.Contracts.Dtos.Auth;
using Auth.Utils;
using FluentValidation;
using Shared.Validation.FluentValidation;

namespace Auth.Validation;

class RegisterDtoValidator : AbstractValidator<RegisterDto>
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