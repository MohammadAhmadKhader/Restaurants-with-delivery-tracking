using Auth.Contracts.Dtos.User;
using Refit;
using Shared.Contracts.Dtos;

namespace Auth.Contracts.Clients;

public interface IUsersServiceClient
{
    [Get("/api/users")]
    Task<CollectionResponse<UserWithRolesDto>> GetUsersAsync([Query] UsersFilterParams filterParams);

    [Get("/api/users/{id}")]
    Task<UserResponseWrapper> GetUserByIdAsync(Guid id);

    [Patch("/api/users/delete/{id}")]
    Task<ApiResponse<object>> DeleteUserByIdAsync(Guid id);

    [Patch("/api/users/self-delete")]
    Task<ApiResponse<object>> SelfDeleteAsync();

    [Put("/api/users/profile")]
    Task<ApiResponse<object>> UpdateProfileAsync([Body] UserUpdateProfile dto);
}

public class UserResponseWrapper
{
    public UserWithRolesDto User { get; set; } = default!;
}