namespace Locations.Endpoints;
public static class LocationsEndpoints
{
    public static void MapLocationsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/locations");

        group.MapGet("/{id}", async (int id) =>
        {

        });

        group.MapPost("/", async (Object dto) =>
        {

        });
    }
}