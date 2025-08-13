using Auth.Contracts.Dtos.Auth;
using Auth.Utils;
using FluentValidation;
using Shared.Validation.FluentValidation;

namespace Auth.Validation;

class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmptyString()
            .EmailAddress()
            .MaximumLength(Constants.MaxEmailLength);

        RuleFor(x => x.Password)
            .NotEmptyString()
            .Length(Constants.MinPasswordLength, Constants.MaxPasswordLength);
    }
}