using Auth.Services.IServices;
using Shared.Filters;
using Auth.Mappers;
using System.Security.Claims;
using Auth.Utils;
using Shared.Utils;
using Auth.Contracts.Dtos.Auth;
using Restaurants.Contracts.Clients;
using Auth.Contracts.Clients;
using Microsoft.AspNetCore.Mvc;

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
                return ResponseUtils.Error("Missing refresh-token");
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
                return ResponseUtils.Forbidden();
            }
            var userIdGuid = Guid.Parse(userId);

            var user = await usersService.FindByIdWithRolesAndPermissionsAsync(userIdGuid);

            return Results.Ok(new { user = user!.ToViewWithRolesAndPermissionsDto() });
        }).RequireAuthorization();

        group.MapPost("/reset-password", async (IAuthService authService, ResetPasswordDto dto, ClaimsPrincipal principal) =>
        {
            var userId = SecurityUtils.ExtractUserId(principal);
            if (userId == Guid.Empty)
            {
                return ResponseUtils.Forbidden();
            }

            await authService.ChangePassword(userId, dto);

            return Results.NoContent();
        }).RequireAuthorization()
        .AddEndpointFilter<ValidationFilter<ResetPasswordDto>>();

        group.MapGet("/test", async ([FromServices] IRestaurantServiceClient restaurantServiceClient,[FromServices] IUsersServiceClient usersServiceClient) =>
        {
            var resp = await restaurantServiceClient.TestPostAsync(new { data = "some data" });
            var userId = Guid.Parse("0196ff3e-43ca-7f6c-8569-c5413f4dd5dd");
            var userResp = await usersServiceClient.GetUserByIdAsync(userId);
      
            return Results.Ok(new { testResp = resp, user = userResp.User });
        });
        
        group.MapGet("/claims", (ClaimsPrincipal principal, ITokenService tokenService) =>
        {
            var claims = tokenService.GetUserClaims(principal);
            return Results.Ok(claims);
        }).RequireAuthorization();
    }
}