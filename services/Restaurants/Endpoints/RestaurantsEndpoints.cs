namespace Restaurants.Endpoints;
public static class RestaurantsEndpoints
{
    public static void MapRestaurantsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/restaurants");

        group.MapGet("/{id}", async (Guid id) =>
        {

        });

        group.MapPost("/", async (Object dto) =>
        {

        });
    }
}