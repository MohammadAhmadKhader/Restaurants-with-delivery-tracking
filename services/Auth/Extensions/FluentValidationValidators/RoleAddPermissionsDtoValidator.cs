using Auth.Contracts.Dtos.Role;
using FluentValidation;

namespace Auth.Extensions.FluentValidationValidators;
class RoleAddPermissionsDtoValidator : AbstractValidator<RoleAddPermissionsDto>
{
    public RoleAddPermissionsDtoValidator()
    {
        RuleFor(x => x.Ids)
        .NotEmpty()
        .WithMessage("at least on permission id is required");
    }
}