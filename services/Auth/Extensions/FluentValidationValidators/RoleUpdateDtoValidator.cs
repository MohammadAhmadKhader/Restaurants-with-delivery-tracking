using Auth.Dtos.Role;
using FluentValidation;

namespace Auth.Extensions.FluentValidationValidators;
class RoleUpdateDtoValidator: AbstractValidator<RoleUpdateDto>
{
    private const int minLength = 3;
    private const int maxLength = 36;
    public RoleUpdateDtoValidator()
    {
        RuleFor(r => r.DisplayName)
        .Length(minLength, maxLength)
        .WithMessage("Display name must be between 3 and 36 characters.");

        RuleFor(r => r.Name)
        .Length(minLength, maxLength)
        .WithMessage("Name must be between 3 and 36 characters.");

        RuleFor(r => r)
        .Must(r => !string.IsNullOrWhiteSpace(r.DisplayName) || !string.IsNullOrWhiteSpace(r.Name))
        .WithName("role")
        .WithMessage("At least one of 'Name' or 'DisplayName' must be provided.");
    }
}