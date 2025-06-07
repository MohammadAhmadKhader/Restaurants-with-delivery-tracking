using Auth.Dtos.Address;
using Auth.Utils;
using FluentValidation;

namespace Auth.Extensions.FluentValidationValidators;

class AddressUpdateDtoValidator: AbstractValidator<AddressUpdateDto>
{
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
        .WithName("address")
        .WithMessage("At least one of 'City' or 'Country' or 'AddressLine' or 'State' or 'PostalCode' must be provided.");;
    }
}