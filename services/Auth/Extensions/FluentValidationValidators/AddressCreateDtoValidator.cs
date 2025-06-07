using Auth.Dtos.Address;
using Auth.Utils;
using FluentValidation;

namespace Auth.Extensions.FluentValidationValidators;
class AddressCreateDtoValidator: AbstractValidator<AddressCreateDto>
{
    public AddressCreateDtoValidator()
    {
        RuleFor(a => a.City)
        .NotEmpty().WithMessage("City is required.")
        .Length(Constants.MinCityLength, Constants.MaxCityLength);

        RuleFor(a => a.Country)
        .NotEmpty().WithMessage("Country is required.")
        .Length(Constants.MinCountryLength, Constants.MaxCountryLength);


        RuleFor(a => a.AddressLine)
        .NotEmpty().WithMessage("AddressLine is required.")
        .Length(Constants.MinAddressLineLength, Constants.MaxAddressLineLength);

        RuleFor(a => a.State)
        .Length(Constants.MinStateLength, Constants.MaxStateLength);

        RuleFor(a => a.PostalCode)
        .NotEmpty().WithMessage("PostalCode is required.")
        .Length(Constants.MinPostalCodeLength, Constants.MaxPostalCodeLength);
    }
}