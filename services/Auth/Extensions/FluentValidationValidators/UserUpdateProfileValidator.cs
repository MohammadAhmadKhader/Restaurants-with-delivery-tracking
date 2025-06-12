using Auth.Dtos.User;
using Auth.Models;
using Auth.Utils;
using FluentValidation;
using Shared.Common;
using Shared.Extensions;

namespace Auth.Extensions.FluentValidationValidators;

class UserUpdateProfileValidator: AbstractValidator<UserUpdateProfile>
{
    public static string AtLeastOneRequiredErrorMessage { get; set; } = default!;
    static UserUpdateProfileValidator()
    {
        var atLeastOneRequiredErrMsg = ValidationMessagesBuilder.AtLeastOneRequired(
            nameof(UserUpdateProfile.FirstName),
            nameof(UserUpdateProfile.LastName));

        AtLeastOneRequiredErrorMessage = atLeastOneRequiredErrMsg;
    }
    public UserUpdateProfileValidator()
    {
        
        RuleFor(x => x.FirstName)
            .NotEmptyString()
            .Length(Constants.MinFirstNameLength, Constants.MaxFirstNameLength);

        RuleFor(x => x.LastName)
            .NotEmptyString()
            .Length(Constants.MinLastNameLength, Constants.MaxLastNameLength);

        RuleFor(r => r)
        .Must(r => !string.IsNullOrWhiteSpace(r.FirstName) || !string.IsNullOrWhiteSpace(r.LastName))
        .WithName(nameof(User))
        .WithMessage(AtLeastOneRequiredErrorMessage);
    }
}