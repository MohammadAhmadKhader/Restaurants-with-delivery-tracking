using Auth.Dtos.Auth;
using Auth.Utils;
using FluentValidation;

namespace Auth.Extensions.FluentValidationValidators;

class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress()
            .MaximumLength(64);
        
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .Length(Constants.MinPasswordLength, Constants.MaxPasswordLength).WithMessage("Password must be between 6 and 36 characters.");
    }
}