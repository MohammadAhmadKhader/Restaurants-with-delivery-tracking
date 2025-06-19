using Auth.Contracts.Dtos.Role;
using Auth.Models;
using Auth.Utils;
using FluentValidation;
using Shared.Common;

namespace Auth.Extensions.FluentValidationValidators;
class RoleUpdateDtoValidator: AbstractValidator<RoleUpdateDto>
{
    public static string AtLeastOneRequiredErrorMessage { get; set; } = default!;
    static RoleUpdateDtoValidator()
    {
        var atLeastOneRequiredErrMsg = ValidationMessagesBuilder.AtLeastOneRequired(
            nameof(RoleCreateDto.Name),
            nameof(RoleCreateDto.DisplayName));

        AtLeastOneRequiredErrorMessage = atLeastOneRequiredErrMsg;
    }
    public RoleUpdateDtoValidator()
    {
        RuleFor(r => r.DisplayName)
        .Length(Constants.MinRoleNameLength, Constants.MaxRoleNameLength);

        RuleFor(r => r.Name)
        .Length(Constants.MinRoleNameLength, Constants.MaxRoleNameLength);

        RuleFor(r => r)
        .Must(r => !string.IsNullOrWhiteSpace(r.DisplayName) || !string.IsNullOrWhiteSpace(r.Name))
        .WithName(nameof(Role))
        .WithMessage(AtLeastOneRequiredErrorMessage);
    }
}