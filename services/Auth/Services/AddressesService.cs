using Auth.Contracts.Dtos.Address;
using Auth.Mappers;
using Auth.Models;
using Auth.Repositories.IRepositories;
using Auth.Services.IServices;
using Shared.Exceptions;

namespace Auth.Services;

public class AddressesService(IUnitOfWork unitOfWork, IAddressesRepository addressesRepository) : IAddressesService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IAddressesRepository _addressesRepository = addressesRepository;
    private const string resourceName = "address";
    public async Task<(List<Address> addresses, int count)> FindAllByUserIdAsync(Guid userId, int page, int limit)
    {
        return await _addressesRepository.FindAllByUserIdAsync(userId, page, limit);
    }
    public async Task<Address?> FindByIdAsync(Guid id)
    {
        return await _addressesRepository.FindByIdAsync(id);
    }
    public async Task<Address> CreateAsync(Guid userId, AddressCreateDto dto)
    {
        var address = new Address
        {
            UserId = userId,
            City = dto.City,
            Country = dto.Country,
            AddressLine = dto.AddressLine,
            PostalCode = dto.PostalCode,
            State = dto.State,
            Longitude = (decimal) dto.Longitude!,
            Latitude = (decimal) dto.Latitude!,
        };

        var newAddress = await _addressesRepository.CreateAsync(address);
        await _unitOfWork.SaveChangesAsync();

        return newAddress;
    }

    public async Task<Address> UpdateAsync(Guid id, Guid userId, AddressUpdateDto dto)
    {
        var address = await this.FindByIdAsync(id);
        if (address == null || address.UserId != userId)
        {
            throw new ResourceNotFoundException(resourceName);
        }

        dto.PatchModel(address);
        await _unitOfWork.SaveChangesAsync();

        return address;
    }

    public async Task DeleteByIdAsync(Guid id, Guid userId)
    {
        var address = await this.FindByIdAsync(id);
        if (address == null || address.UserId != userId)
        {
            throw new ResourceNotFoundException(resourceName);
        }

        _addressesRepository.Delete(address);
        await _unitOfWork.SaveChangesAsync();
    }
}