using Auth.Contracts.Clients;
using Microsoft.AspNetCore.Mvc;
using Restaurants.Contracts.Dtos.Restaurant;
using Restaurants.Mappers;
using Restaurants.Services.IServices;
using Shared.Auth;
using Shared.Constants;
using Shared.Utils;

namespace Restaurants.Endpoints;
public static class RestaurantsEndpoints
{
    public static void MapRestaurantsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/restaurants");

        group.MapGet("/{id}", async (Guid id, IRestaurantsService restaurantsService) =>
        {
            var rest = await restaurantsService.FindByIdAsync(id);
            if (rest == null)
            {
                return ResponseUtils.NotFound("restaurant");
            }

            return Results.Ok(new { restaurant = rest.ToViewDto() });
        });

        group.MapPut("/", async (RestaurantUpdateDto dto, IRestaurantsService restaurantsService) =>
        {
            var invitation = await restaurantsService.UpdateAsync(dto);

            return Results.NoContent();
        }).RequirePermission(RestaurantPermissions.RESTAURANT_UPDATE);

        group.MapPost("/invitations/send", async (
            RestaurantInvitationCreateDto dto,
            IRestaurantInvitationsService restaurantInvitationsService,
            IAuthServiceClient authServiceClient) =>
        {
            var userInfo = await authServiceClient.GetUserInfoAsync();
            var invitation = await restaurantInvitationsService.CreateAsync(dto.Email, userInfo.UserId);

            return Results.Ok(new { invitation = invitation.ToViewDto() });
        });

        group.MapGet("/invitations", async ([FromQuery] string? invId, IRestaurantInvitationsService invitationsService) =>
        {
            if (string.IsNullOrWhiteSpace(invId))
            {
                return ResponseUtils.BadRequest("token is required.");
            }

            var invitation = await invitationsService.InvitiationExistsAsync(invId);
            if (!invitation)
            {
                return ResponseUtils.NotFound("restaurant-invitation");
            }

            return Results.Ok();
        });

        group.MapPost("/invitations/accept", async (
            RestaurantInvitationAcceptDto dto,
            ILogger<Program> logger,
            IRestaurantInvitationsService restaurantInvitationsService,
            IRestaurantsService restaurantsService) =>
        {
            logger.LogInformation("Creating restaurant {@RestaurantInvitationAcceptDto}", dto);
            var restaurant = await restaurantsService.CreateAsync(dto);
            logger.LogInformation("Restaurant created successfully");

            return Results.Ok(new { restaurant = restaurant.ToViewDto() });
        });

        group.MapGet("/test", () =>
        {
            return Results.Ok(new { response = "responses" });
        });
    }
}