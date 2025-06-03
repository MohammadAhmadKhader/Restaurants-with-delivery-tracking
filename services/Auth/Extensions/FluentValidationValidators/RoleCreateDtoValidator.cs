using Auth.Dtos.Role;
using FluentValidation;

namespace Auth.Extensions.FluentValidationValidators;
class RoleCreateDtoValidator: AbstractValidator<RoleCreateDto>
{
    private const int minLength = 3;
    private const int maxLength = 36;
    public RoleCreateDtoValidator()
    {
        RuleFor(x => x.DisplayName)
        .NotEmpty().WithMessage("Display name is required.")
        .Length(minLength, maxLength)
        .WithMessage("Display name must be between 3 and 36 characters.");
        
        RuleFor(x => x.Name)
        .NotEmpty().WithMessage("Name is required.")
        .Length(minLength, maxLength)
        .WithMessage("Name must be between 3 and 36 characters.");
    }
}