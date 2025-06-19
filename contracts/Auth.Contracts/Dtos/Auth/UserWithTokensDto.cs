using Auth.Contracts.Dtos.User;

namespace Auth.Contracts.Dtos.Auth;

public record UserWithTokensDto(UserWithRolesAndPermissionsDto User, string AccessToken, string RefreshToken);