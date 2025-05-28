using Auth.Dtos.Auth;
using FluentValidation;

namespace Auth.Extensions.FluentValidationValidators;
class ResetPasswordDtoValidator : AbstractValidator<ResetPasswordDto>
{
    public ResetPasswordDtoValidator()
    {
        RuleFor(x => x.ConfirmNewPassword)
            .NotEmpty().WithMessage("ConfirmNewPassword is required.")
            .Length(6, 36).WithMessage("ConfirmNewPassword must be between 6 and 36 characters.")
            .Equal(x => x.NewPassword).WithMessage("Passwords mismatch.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("NewPassword is required.")
            .Length(6, 36).WithMessage("NewPassword must be between 6 and 36 characters.");

        RuleFor(x => x.OldPassword)
            .NotEmpty().WithMessage("OldPassword is required.")
            .Length(6, 36).WithMessage("OldPassword must be between 6 and 36 characters.");
    }
}