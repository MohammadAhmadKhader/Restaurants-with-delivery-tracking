using Auth.Contracts.Dtos.Auth;
using Auth.Utils;
using FluentValidation;
using Shared.Extensions;

namespace Auth.Extensions.FluentValidationValidators;
class ResetPasswordDtoValidator : AbstractValidator<ResetPasswordDto>
{
    public ResetPasswordDtoValidator()
    {
        RuleFor(x => x.ConfirmNewPassword)
            .NotEmptyString()
            .Length(Constants.MinPasswordLength, Constants.MaxPasswordLength)
            .Equal(x => x.NewPassword).WithMessage("Passwords mismatch.");

        RuleFor(x => x.NewPassword)
            .NotEmptyString()
            .Length(Constants.MinPasswordLength, Constants.MaxPasswordLength);

        RuleFor(x => x.OldPassword)
            .NotEmptyString()
            .Length(Constants.MinPasswordLength, Constants.MaxPasswordLength);
    }
}