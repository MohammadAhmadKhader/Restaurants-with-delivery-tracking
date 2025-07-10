using System.Security.Claims;
using Auth.Contracts.Clients;
using Auth.Contracts.Dtos.Auth;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Restaurants.Contracts.Dtos;
using Restaurants.Mappers;
using Restaurants.Services.IServices;
using Shared.Kafka;
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

        // group.MapPost("/", async (RestaurantCreateDto dto, [FromQuery] string? token, IRestaurantsService restaurantsService) =>
        // {
        //     var newResturat = await restaurantsService.CreateAsync(dto, token);

        //     return Results.Ok(new { restaurant = newResturat.ToViewDto() });
        // });

        group.MapPost("/send-invitation", async (
            RestaurantInvitationCreateDto dto,
            IRestaurantInvitationsService restaurantInvitationsService,
            IAuthServiceClient authServiceClient) =>
        {
            var claims = await authServiceClient.Claims();
            var invitation = await restaurantInvitationsService.CreateAsync(dto.Email, claims.UserId);

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

        group.MapPost("/accept-invitation", async (
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