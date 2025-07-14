using Auth.Services.IServices;
using Shared.Filters;
using Auth.Mappers;
using System.Security.Claims;
using Auth.Utils;
using Shared.Utils;
using Auth.Contracts.Dtos.Auth;
using MassTransit;
using Shared.Kafka;
using Refit;
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

        appGroup.MapGet("/claims", (ClaimsPrincipal principal, ITokenService tokenService) =>
        {
            var claims = tokenService.GetUserClaims(principal);
            return Results.Ok(claims);
        }).RequireAuthorization();

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