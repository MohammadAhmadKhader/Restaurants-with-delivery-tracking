using Refit;
using Restaurants.Contracts.Dtos.Restaurant;

namespace Restaurants.Contracts.Clients;

public interface IRestaurantServiceClient
{
    [Get("/api/restaurants/{id}")]
    Task<RestaurantResponseWrappaer> GetRestaurantById(Guid id);

    [Post("/api/restaurants")]
    Task<RestaurantResponseWrappaer> CreateRestaurant([Body] RestaurantCreateDto dto, [Query] string token);

    [Post("/api/restaurants/send-invitation")]
    Task<RestaurantInvitationResponseWrappaer> SendInvitation([Body] RestaurantInvitationCreateDto dto);

    [Get("/api/restaurants/invitations")]
    Task<ApiResponse<object>> CheckInvitationExistence([Query] string token);

    [Post("/api/restaurants/accept-invitation")]
    Task<RestaurantResponseWrappaer> AcceptInvitation([Body] RestaurantInvitationAcceptDto dto, [Query] string token);

    [Post("/api/restaurants/test")]
    Task<TestBody> TestPostAsync([Body] object data);
}

public class TestBody
{
    public Dictionary<string, object> Body { get; set; } = default!;
}

public class RestaurantResponseWrappaer
{
    public RestaurantViewDto Restaurant { get; set; } = default!;
}

public class RestaurantInvitationResponseWrappaer
{
    public RestaurantInvitationViewDto Invitation { get; set; } = default!;
}