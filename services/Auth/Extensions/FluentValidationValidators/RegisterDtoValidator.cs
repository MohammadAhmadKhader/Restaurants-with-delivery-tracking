using Auth.Dtos;
using Auth.Dtos.Auth;
using FluentValidation;

namespace Auth.Extensions.FluentValidationValidators;

class RegisterDtoValidator: AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .Length(3, 36).WithMessage("First name must be between 3 and 36 characters.");
            
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .Length(3, 36).WithMessage("Last name must be between 3 and 36 characters.");
        
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email.")
            .MaximumLength(64).WithMessage("Email must be at most 64 characters.");
    
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}
    
