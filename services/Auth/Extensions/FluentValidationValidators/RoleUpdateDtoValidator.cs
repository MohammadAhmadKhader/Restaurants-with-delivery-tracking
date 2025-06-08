using Auth.Dtos.Role;
using Auth.Utils;
using FluentValidation;
using Shared.Common;

namespace Auth.Extensions.FluentValidationValidators;
class RoleUpdateDtoValidator: AbstractValidator<RoleUpdateDto>
{
    public RoleUpdateDtoValidator()
    {
        var atLeastOneRequiredErrMsg = ValidationMessagesBuilder.AtLeastOneRequired(
            nameof(RoleCreateDto.Name),
            nameof(RoleCreateDto.DisplayName));
        RuleFor(r => r.DisplayName)
        .Length(Constants.MinRoleNameLength, Constants.MaxRoleNameLength);

        RuleFor(r => r.Name)
        .Length(Constants.MinRoleNameLength, Constants.MaxRoleNameLength);

        RuleFor(r => r)
        .Must(r => !string.IsNullOrWhiteSpace(r.DisplayName) || !string.IsNullOrWhiteSpace(r.Name))
        .WithName("role")
        .WithMessage(atLeastOneRequiredErrMsg);
    }
}