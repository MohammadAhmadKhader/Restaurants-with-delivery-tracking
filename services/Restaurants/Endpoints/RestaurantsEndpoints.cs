using Auth.Contracts.Clients;
using Microsoft.AspNetCore.Mvc;
using Restaurants.Contracts.Dtos;
using Restaurants.Mappers;
using Restaurants.Services.IServices;
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
            IRestaurantInvitationsService restaurantInvitationsService) =>
        {
            var ownerId = Guid.NewGuid();
            var invitation = await restaurantInvitationsService.CreateAsync(dto.Email, ownerId);

            return Results.Ok(new { invitation = invitation.ToViewDto() });
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
            IRestaurantsService restaurantsService,
            IAuthServiceClient authServiceClient) =>
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return ResponseUtils.BadRequest("token is required.");
            }
            
            var newUser = await authServiceClient.RegisterAsync(dto.User);
            var restaurant = await restaurantsService.CreateAsync(dto.Restaurant, token, newUser.User.Id);

            return Results.Ok(new { restaurant = restaurant.ToViewDto() });
        });

        group.MapPost("/test", (object body) =>
        {
            return Results.Ok(new { body });
        });
    }
}