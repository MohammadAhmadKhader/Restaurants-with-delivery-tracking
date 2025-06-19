using Auth.Contracts.Dtos.Address;
using Auth.Models;
using Auth.Utils;
using FluentValidation;
using Shared.Common;

namespace Auth.Extensions.FluentValidationValidators;

class AddressUpdateDtoValidator: AbstractValidator<AddressUpdateDto>
{
    public static string AtLeastOneRequiredErrorMessage { get; set; } = default!;
    static AddressUpdateDtoValidator()
    {
        var atLeastOneRequiredErrMsg = ValidationMessagesBuilder.AtLeastOneRequired(
            nameof(Address.City),
            nameof(Address.Country),
            nameof(Address.State),
            nameof(Address.AddressLine),
            nameof(Address.PostalCode));

        AtLeastOneRequiredErrorMessage = atLeastOneRequiredErrMsg;
    }

    public AddressUpdateDtoValidator()
    {
        RuleFor(a => a.City)
        .Length(Constants.MinCityLength, Constants.MaxCityLength);

        RuleFor(a => a.Country)
        .Length(Constants.MinCountryLength, Constants.MaxCountryLength);

        RuleFor(a => a.AddressLine)
        .Length(Constants.MinAddressLineLength, Constants.MaxAddressLineLength);

        RuleFor(a => a.State)
        .Length(Constants.MinStateLength, Constants.MaxStateLength);

        RuleFor(a => a.PostalCode)
        .Length(Constants.MinPostalCodeLength, Constants.MaxPostalCodeLength);

        RuleFor(a => a)
        .Must(a => a.City != null || a.Country != null || a.AddressLine != null || a.State != null || a.PostalCode != null)
        .WithName(nameof(Address))
        .WithMessage(AtLeastOneRequiredErrorMessage);
    }
}