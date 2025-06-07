using Auth.Dtos.Role;
using Auth.Utils;
using FluentValidation;

namespace Auth.Extensions.FluentValidationValidators;
class RoleCreateDtoValidator: AbstractValidator<RoleCreateDto>
{
    public RoleCreateDtoValidator()
    {
        RuleFor(x => x.DisplayName)
        .NotEmpty().WithMessage("DisplayName is required.")
        .Length(Constants.MinRoleNameLength, Constants.MaxRoleNameLength);

        RuleFor(x => x.Name)
        .NotEmpty().WithMessage("Name is required.")
        .Length(Constants.MinRoleNameLength, Constants.MaxRoleNameLength);
    }
}