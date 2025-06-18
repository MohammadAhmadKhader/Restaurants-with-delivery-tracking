using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Restaurants.Dtos;
using Restaurants.Services.IServices;
using Shared.Utils;
using Xunit.Sdk;

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

            return Results.Ok(new { restaurant = rest });
        });

        group.MapPost("/", async (RestaurantCreateDto dto, [FromQuery] string? token, IRestaurantsService restaurantsService) =>
        {
            var newResturat = await restaurantsService.CreateAsync(dto, token);

            return Results.Ok(new { restaurant = newResturat });
        });

        group.MapPost("/send-invitation", async (
            RestaurantInvitationCreateDto dto,
            IRestaurantInvitationsService restaurantInvitationsService) =>
        {
            var userId = Guid.NewGuid();
            var invitation = await restaurantInvitationsService.CreateAsync(dto.Email, userId);

            return Results.Ok(new { invitation });
        });

        group.MapGet("/invitations", async ([FromQuery] string? token, IRestaurantInvitationsService invitationsService) =>
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return ResponseUtils.BadRequest("token is required.");
            }

            var invitation = await invitationsService.InvitiationExistsAsync(token);
            if (!invitation)
            {
                return ResponseUtils.NotFound("restaurant-invitation");
            }

            return Results.Ok();
        });

        group.MapPost("/accept-invitation", async (
            RestaurantInvitationAcceptDto dto,
            [FromQuery] string? token,
            IRestaurantInvitationsService restaurantInvitationsService,
            IRestaurantsService restaurantsService) =>
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return ResponseUtils.BadRequest("token is required.");
            }

            var restaurant = await restaurantsService.CreateAsync(null!, token!);

            return Results.Ok(new { restaurant });
        });

        group.MapPost("/test", async (object body) =>
        {
            return Results.Ok(new { body });
        });
    }
}