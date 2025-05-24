using Auth.Dtos;
using FluentValidation;

namespace Auth.Extensions.FluentValidation.Validators;

class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email.")
            .MaximumLength(64).WithMessage("Email must be at most 64 characters.");
        
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}
