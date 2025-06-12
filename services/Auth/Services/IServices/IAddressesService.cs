using Auth.Dtos.Address;
using Auth.Dtos.Auth;
using Auth.Models;

namespace Auth.Services.IServices;

public interface IAddressesService
{
    Task<(List<Address> addresses, int count)> FindAllByUserIdAsync(Guid userId, int page, int limit);
    Task<Address?> FindByIdAsync(Guid id);
    Task<Address> CreateAsync(Guid userId, AddressCreateDto dto);
    Task<Address> UpdateAsync(Guid id, Guid userId, AddressUpdateDto address);
    Task DeleteByIdAsync(Guid id, Guid userId);
}