using Auth.Dtos.Address;
using Auth.Models;
using Auth.Utils;
using FluentValidation;
using Shared.Common;
using Shared.Constants;
using Shared.Utils;

namespace Auth.Extensions.FluentValidationValidators;

class AddressUpdateDtoValidator: AbstractValidator<AddressUpdateDto>
{
    public AddressUpdateDtoValidator()
    {
        var atLeastOneRequiredErrMsg = ValidationMessagesBuilder.AtLeastOneRequired(
            nameof(Address.City),
            nameof(Address.Country),
            nameof(Address.AddressLine),
            nameof(Address.PostalCode));
            
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
        .WithName("address")
        .WithMessage(atLeastOneRequiredErrMsg);
    }
}