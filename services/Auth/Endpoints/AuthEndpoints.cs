using Auth.Services.IServices;
using Shared.Filters;
using Auth.Mappers;
using System.Security.Claims;
using Auth.Utils;
using Shared.Utils;
using Auth.Contracts.Dtos.Auth;
using Shared.Tenant;
using Notifications.Contracts.Clients;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var appGroup = app.MapGroup("/api/auth");

        appGroup.MapPost("/login", async (LoginDto dto, IAuthService authService) =>
        {
            var (user, tokenData) = await authService.Login(dto);
            var resBody = new UserWithTokensDto(
                user.ToViewWithRolesAndPermissionsDto(),
                tokenData.AccessToken,
                tokenData.RefreshToken);

            return Results.Ok(resBody);
        }).AddEndpointFilter<ValidationFilter<LoginDto>>();

        appGroup.MapPost("/register", async (RegisterDto dto, IAuthService authService) =>
        {
            var (user, tokenData) = await authService.Register(dto);
            var resBody = new UserWithTokensDto(
                user.ToViewWithRolesAndPermissionsDto(),
                tokenData.AccessToken,
                tokenData.RefreshToken);

            return Results.Json(resBody, statusCode: StatusCodes.Status201Created);
        }).AddEndpointFilter<ValidationFilter<RegisterDto>>();

        appGroup.MapPost("/refresh", async (RefreshRequest req, ITokenService tokenService) =>
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

        appGroup.MapGet("/user", async (HttpContext http, IUsersService usersService, ILogger<Program> logger) =>
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

        appGroup.MapPost("/reset-password", async (IAuthService authService, ResetPasswordDto dto, ClaimsPrincipal principal) =>
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

        appGroup.MapGet("/user-info", async (ClaimsPrincipal principal, ITokenService tokenService) =>
        {
            var userInfo = await tokenService.GetUserInfoAsync(principal);

            return Results.Ok(userInfo);
        }).RequireAuthorization();

        appGroup.MapGet("/user-details", async (ClaimsPrincipal principal, ITokenService tokenService) =>
        {
            var userDetails = await tokenService.GetUserDetailsAsync(principal);

            return Results.Ok(userDetails);
        }).RequireAuthorization();

        appGroup.MapPost("/forgot-password/send", async (
            ForgotPasswordDto dto,
            IUsersService authService,
            IPasswordResetTokensService passwordResetTokensService,
            INotificationsServiceClient notificationsServiceClient) =>
        {
            const string responseMessage = "If email exists, reset instructions have been sent.";
            var user = await authService.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return Results.Ok(new { message = responseMessage });
            }

            var newToken = await passwordResetTokensService.CreateAsync(user.Id);
            await notificationsServiceClient.SendForgotPasswordEmail(new(user.Email!, newToken.Token));

            return Results.Ok(new { message = responseMessage });
        });
        
        appGroup.MapPost("/forgot-password/reset", async (
            ResetPasswordDto dto,
            [FromQuery] string? token,
            IAuthService authService,
            IPasswordResetTokensService passwordResetTokensService) =>
        {
            if (token == null)
            {
                return ResponseUtils.BadRequest("Invalid token.");
            }

            var foundToken = await passwordResetTokensService.FindByTokenAsync(token);
            if (foundToken == null)
            {
                
                return ResponseUtils.BadRequest("Invalid token.");
            }

            await authService.ChangePassword(foundToken.UserId, dto);
            await passwordResetTokensService.MarkAsUsedAsync(foundToken);

            return Results.NoContent();
        })
        .AddEndpointFilter<ValidationFilter<ResetPasswordDto>>();

        var restaurantGroup = app.MapGroup("/api/auth/restaurants");
        restaurantGroup.MapPost("/login", async (LoginDto dto, IAuthService authService, ITenantProvider tenantProvider) =>
        {
            tenantProvider.GetTenantIdOrThrow();

            var (user, tokenData) = await authService.LoginRestaurant(dto);
            var resBody = new RestaurantUserWithTokensDto(
                user.ToViewWithRestaurantRolesAndPermissionsDto(),
                tokenData.AccessToken,
                tokenData.RefreshToken);

            return Results.Ok(resBody);
        }).AddEndpointFilter<ValidationFilter<LoginDto>>();

        restaurantGroup.MapPost("/register", async (RegisterDto dto, IAuthService authService, ITenantProvider tenantProvider) =>
        {
            var restId = tenantProvider.GetTenantIdOrThrow();

            var (user, tokenData) = await authService.RegisterRestaurant(dto, restId);
            var resBody = new RestaurantUserWithTokensDto(
                user.ToViewWithRestaurantRolesAndPermissionsDto(),
                tokenData.AccessToken,
                tokenData.RefreshToken);

            return Results.Json(resBody, statusCode: StatusCodes.Status201Created);
        }).AddEndpointFilter<ValidationFilter<RegisterDto>>();


        // appGroup.MapGet("/test-kafka", async ([Query] string? value, ITopicProducer<SimpleTestEvent> producer) =>
        // {
        //     var sentEvent = new SimpleTestEvent(value ?? "No value provided");
        //     await producer.Produce(sentEvent);
        //     return Results.Ok(new { sentEvent });
        // });
    }
}