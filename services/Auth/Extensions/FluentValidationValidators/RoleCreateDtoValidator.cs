using Auth.Dtos.Role;
using Auth.Utils;
using FluentValidation;

namespace Auth.Extensions.FluentValidationValidators;
class RoleCreateDtoValidator: AbstractValidator<RoleCreateDto>
{
    public RoleCreateDtoValidator()
    {
        RuleFor(x => x.DisplayName)
        .NotEmpty().WithMessage("Display name is required.")
        .Length(Constants.MinRoleNameLength, Constants.MaxRoleNameLength)
        .WithMessage("Display name must be between 3 and 36 characters.");
        
        RuleFor(x => x.Name)
        .NotEmpty().WithMessage("Name is required.")
        .Length(Constants.MinRoleNameLength, Constants.MaxRoleNameLength)
        .WithMessage("Name must be between 3 and 36 characters.");
    }
}