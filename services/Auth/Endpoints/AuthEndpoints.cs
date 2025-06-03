using Auth.Services.IServices;
using Shared.Filters;
using Auth.Mappers;
using System.Security.Claims;
using Auth.Dtos.Auth;
using Auth.Utils;

namespace Auth.Endpoints;
public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");

        group.MapPost("/login", async (LoginDto dto, IAuthService authService) =>
        {
            var (user, tokenData) = await authService.Login(dto);
            var resBody = new UserWithTokensDto(
                user.ToViewWithRolesAndPermissionsDto(),
                tokenData.AccessToken,
                tokenData.RefreshToken);

            return Results.Ok(resBody);
        }).AddEndpointFilter<ValidationFilter<LoginDto>>();

        group.MapPost("/register", async (RegisterDto dto, IAuthService authService) =>
        {
            var (user, tokenData) = await authService.Register(dto);
            var resBody = new UserWithTokensDto(
                user.ToViewWithRolesAndPermissionsDto(),
                tokenData.AccessToken,
                tokenData.RefreshToken);

            return Results.Json(resBody, statusCode: StatusCodes.Status201Created);
        }).AddEndpointFilter<ValidationFilter<RegisterDto>>();

        group.MapPost("/refresh", async (RefreshRequest req, ITokenService tokenService) =>
        {
            var refToken = req.RefreshToken;
            if (refToken == null || string.IsNullOrEmpty(refToken))
            {
                return Results.BadRequest(new { detail = "Missing refresh-token" });
            }

            var tokens = await tokenService.RefreshTokenAsync(refToken);
            var resBody = new RefreshResponseDto(tokens.AccessToken, tokens.RefreshToken);

            return Results.Ok(resBody);
        });

        group.MapGet("/user", async (HttpContext http, IUsersService usersService, ILogger<Program> logger) =>
        {
            var userId = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                const string message = "User id claim was not found in the token.";
                logger.LogError(message);
                return Results.Unauthorized();
            }
            var userIdGuid = Guid.Parse(userId);

            var user = await usersService.FindByIdWithRolesAndPermissionsAsync(userIdGuid);

            return Results.Ok(new { user = user.ToViewWithRolesAndPermissionsDto() });
        }).RequireAuthorization();

        group.MapPost("/reset-password", async (IAuthService authService, ResetPasswordDto dto, ClaimsPrincipal principal) =>
        {
            var userId = SecurityUtils.ExtractUserId(principal);
            if (userId == Guid.Empty)
            {
                return Results.Forbid();
            }

            await authService.ChangePassword(userId, dto);
    
            return Results.NoContent();
        }).RequireAuthorization()
        .AddEndpointFilter<ValidationFilter<ResetPasswordDto>>();
    }
}