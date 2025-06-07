using Auth.Dtos.Auth;
using Auth.Utils;
using FluentValidation;

namespace Auth.Extensions.FluentValidationValidators;
class ResetPasswordDtoValidator : AbstractValidator<ResetPasswordDto>
{
    public ResetPasswordDtoValidator()
    {
        RuleFor(x => x.ConfirmNewPassword)
            .NotEmpty().WithMessage("ConfirmNewPassword is required.")
            .Length(Constants.MinPasswordLength, Constants.MaxPasswordLength)
            .Equal(x => x.NewPassword).WithMessage("Passwords mismatch.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("NewPassword is required.")
            .Length(Constants.MinPasswordLength, Constants.MaxPasswordLength);

        RuleFor(x => x.OldPassword)
            .NotEmpty().WithMessage("OldPassword is required.")
            .Length(Constants.MinPasswordLength, Constants.MaxPasswordLength);
    }
}