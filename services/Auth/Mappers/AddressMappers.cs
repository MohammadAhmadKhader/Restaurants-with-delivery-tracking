using Auth.Contracts.Dtos.Address;
using Auth.Models;

namespace Auth.Mappers;

public static class AdddressMappers
{
    public static AddressViewDto ToViewDto(this Address address)
    {
        return new AddressViewDto
        {
            Id = address.Id,
            UserId = address.UserId,
            City = address.City,
            Country = address.Country,
            AddressLine = address.AddressLine,
            PostalCode = address.PostalCode,
            State = address.State,
            Longitude = address.Longitude,
            Latitude = address.Latitude,
        };
    }

    public static void PatchModel(this AddressUpdateDto dto, Address model)
    {
        if (!string.IsNullOrWhiteSpace(dto.City))
        {
            model.City = dto.City;
        }

        if (!string.IsNullOrWhiteSpace(dto.Country))
        {
            model.Country = dto.Country;
        }

        if (!string.IsNullOrWhiteSpace(dto.AddressLine))
        {
            model.AddressLine = dto.AddressLine;
        }

        if (!string.IsNullOrWhiteSpace(dto.PostalCode))
        {
            model.PostalCode = dto.PostalCode;
        }

        if (!string.IsNullOrWhiteSpace(dto.State))
        {
            model.State = dto.State;
        }
    }
}