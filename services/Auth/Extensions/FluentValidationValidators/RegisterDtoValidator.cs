using Auth.Dtos.Auth;
using Auth.Utils;
using FluentValidation;

namespace Auth.Extensions.FluentValidationValidators;

class RegisterDtoValidator: AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("FirstName is required.")
            .Length(Constants.MinFirstNameLength, Constants.MaxFirstNameLength);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("LastName is required.")
            .Length(Constants.MinLastNameLength, Constants.MaxLastNameLength);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress()
            .MaximumLength(Constants.MaxEmailLength);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .Length(Constants.MinPasswordLength, Constants.MaxPasswordLength);
    }
}