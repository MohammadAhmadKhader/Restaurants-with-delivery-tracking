using Auth.Dtos.Role;
using Auth.Utils;
using FluentValidation;

namespace Auth.Extensions.FluentValidationValidators;
class RoleUpdateDtoValidator: AbstractValidator<RoleUpdateDto>
{
    public RoleUpdateDtoValidator()
    {
        RuleFor(r => r.DisplayName)
        .Length(Constants.MinRoleNameLength, Constants.MaxRoleNameLength);

        RuleFor(r => r.Name)
        .Length(Constants.MinRoleNameLength, Constants.MaxRoleNameLength);

        RuleFor(r => r)
        .Must(r => !string.IsNullOrWhiteSpace(r.DisplayName) || !string.IsNullOrWhiteSpace(r.Name))
        .WithName("role")
        .WithMessage("At least one of 'Name' or 'DisplayName' must be provided.");
    }
}