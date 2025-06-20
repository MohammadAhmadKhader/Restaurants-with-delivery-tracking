using Auth.Contracts.Dtos.Address;
using Refit;
using Shared.Contracts.Dtos;

namespace Auth.Contracts.Clients;

public interface IAddressesServiceClient
{
    [Get("/api/users/addresses")]
    Task<CollectionResponse<AddressViewDto>> GetUserAddresses([Query] PagedRequest pagedRequest);

    [Post("/api/users/addresses")]
    Task<AddressResponseWrapper> CreateAddress([Body] AddressCreateDto dto);

    [Put("/api/users/addresses/{id}")]
    Task<ApiResponse<object>> UpdateAddress(Guid id, [Body] AddressUpdateDto dto);

    [Delete("/api/users/addresses/{id}")]
    Task<ApiResponse<object>> DeleteAddress(Guid id);
}

public class AddressResponseWrapper
{
    public AddressViewDto Address { get; set; } = default!;
}