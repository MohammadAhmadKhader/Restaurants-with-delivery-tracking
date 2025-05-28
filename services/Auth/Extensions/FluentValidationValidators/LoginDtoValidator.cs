using Auth.Dtos;
using Auth.Dtos.Auth;
using FluentValidation;

namespace Auth.Extensions.FluentValidationValidators;

class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email.")
            .MaximumLength(64).WithMessage("Email must be at most 64 characters.");
        
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .Length(6, 36).WithMessage("Password must be between 6 and 36 characters.");
    }
}
