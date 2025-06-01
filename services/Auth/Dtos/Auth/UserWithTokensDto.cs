using System.Text.Json.Serialization;
using Auth.Dtos.User;

namespace Auth.Dtos.Auth;

public record UserWithTokensDto(UserWithRolesAndPermissionsDto User, string AccessToken, string RefreshToken);