using Auth.Contracts.Dtos.Role;
using Auth.Utils;
using FluentValidation;
using Shared.Validation.FluentValidation;

namespace Auth.Validation;

class RoleCreateDtoValidator : AbstractValidator<RoleCreateDto>
{
    public RoleCreateDtoValidator()
    {
        RuleFor(x => x.DisplayName)
        .NotEmptyString()
        .Length(Constants.MinRoleNameLength, Constants.MaxRoleNameLength);

        RuleFor(x => x.Name)
        .NotEmptyString()
        .Length(Constants.MinRoleNameLength, Constants.MaxRoleNameLength);
    }
}