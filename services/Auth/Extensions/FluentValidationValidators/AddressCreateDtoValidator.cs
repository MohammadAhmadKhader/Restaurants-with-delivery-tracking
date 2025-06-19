using Auth.Contracts.Dtos.Address;
using Auth.Utils;
using FluentValidation;

using Shared.Extensions;

namespace Auth.Extensions.FluentValidationValidators;
class AddressCreateDtoValidator: AbstractValidator<AddressCreateDto>
{
    public AddressCreateDtoValidator()
    {
        RuleFor(a => a.City)
        .NotEmptyString()
        .Length(Constants.MinCityLength, Constants.MaxCityLength);

        RuleFor(a => a.Country)
        .NotEmptyString()
        .Length(Constants.MinCountryLength, Constants.MaxCountryLength);

        RuleFor(a => a.AddressLine)
        .NotEmptyString()
        .Length(Constants.MinAddressLineLength, Constants.MaxAddressLineLength);

        RuleFor(a => a.State)
        .Length(Constants.MinStateLength, Constants.MaxStateLength);

        RuleFor(a => a.PostalCode)
        .NotEmptyString()
        .Length(Constants.MinPostalCodeLength, Constants.MaxPostalCodeLength);

        RuleFor(x => x.Latitude)
            .NotNull()
            .InclusiveBetween(Constants.MinLatitude, Constants.MaxLatitude);

        RuleFor(x => x.Longitude)
            .NotNull()
            .InclusiveBetween(Constants.MinLongitude, Constants.MaxLongitude);
    }
}