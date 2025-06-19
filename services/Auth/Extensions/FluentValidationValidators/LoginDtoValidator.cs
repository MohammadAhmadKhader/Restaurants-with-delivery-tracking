using Auth.Contracts.Dtos.Auth;
using Auth.Utils;
using FluentValidation;
using Shared.Extensions;

namespace Auth.Extensions.FluentValidationValidators;

class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmptyString()
            .EmailAddress()
            .MaximumLength(64);
        
        RuleFor(x => x.Password)
            .NotEmptyString()
            .Length(Constants.MinPasswordLength, Constants.MaxPasswordLength);
    }
}