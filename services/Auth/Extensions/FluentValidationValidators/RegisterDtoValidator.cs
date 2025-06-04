using Auth.Dtos.Auth;
using Auth.Utils;
using FluentValidation;

namespace Auth.Extensions.FluentValidationValidators;

class RegisterDtoValidator: AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .Length(Constants.MinFirstNameLength, Constants.MaxFirstNameLength).WithMessage("First name must be between 3 and 36 characters.");
            
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .Length(Constants.MinLastNameLength, Constants.MaxLastNameLength).WithMessage("Last name must be between 3 and 36 characters.");
        
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email.")
            .MaximumLength(Constants.MaxEmailLength).WithMessage("Email must be at most 64 characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .Length(Constants.MinPasswordLength, Constants.MaxPasswordLength).WithMessage("Password must be between 6 and 36 characters.");
    }
}