using Auth.Services.IServices;
using Microsoft.AspNetCore.Mvc;
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

        group.MapPost("/login", async ([FromBody] LoginDto dto, IAuthService authService) =>
        {
            var (user, tokenData) = await authService.Login(dto);
            var resBody = new Dictionary<string, object>
            {
                {"user", user.ToViewWithRolesAndPermissionsDto() },
                {"access-token", tokenData.AccessToken },
                {"refresh-token", tokenData.RefreshToken }
            };

            return Results.Ok(resBody);
        }).AddEndpointFilter<ValidationFilter<LoginDto>>();

        group.MapPost("/register", async ([FromBody] RegisterDto dto, IAuthService authService) =>
        {
            var (user, tokenData) = await authService.Register(dto);
            var resBody = new Dictionary<string, object>
            {
                {"user", user.ToViewWithRolesAndPermissionsDto() },
                {"access-token", tokenData.AccessToken },
                {"refresh-token", tokenData.RefreshToken }
            };

            return Results.Json(resBody, statusCode: StatusCodes.Status201Created);
        }).AddEndpointFilter<ValidationFilter<RegisterDto>>();

        group.MapPost("/refresh", async ([FromBody] RefreshRequest req, ITokenService tokenService) =>
        {
            var refToken = req.RefreshToken;
            if (refToken == null || string.IsNullOrEmpty(refToken))
            {
                return Results.BadRequest(new { detail = "Missing refresh-token" });
            }

            var tokens = await tokenService.RefreshTokenAsync(refToken);
            var resBody = new Dictionary<string, object>
            {
                {"access-token", tokens.AccessToken },
                {"refresh-token", tokens.RefreshToken }
            };

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

            var user = await usersService.FindByIdWithRolesAndPermissions(userIdGuid);
            if (user == null)
            {
                const string message = "User was not found despite it was authenticated and its id was fetched from the token.";
                logger.LogError(message);
                return Results.Unauthorized();
            }

            return Results.Ok(new { user = user.ToViewWithRolesAndPermissionsDto() });
        }).RequireAuthorization();

        group.MapPost("/reset-password", async (IAuthService authService, ResetPasswordDto dto, ClaimsPrincipal principal) =>
        {
            var userId = SecurityUtils.ExtractUserId(principal);
            if (userId == Guid.Empty)
            {
                return Results.Forbid();
            }

            var (success, error) = await authService.ChangePassword(userId, dto);
            if (!success)
            {
                return Results.BadRequest(new { detail = error });
            }

            return Results.NoContent();
        }).RequireAuthorization()
        .AddEndpointFilter<ValidationFilter<ResetPasswordDto>>();
    }
}