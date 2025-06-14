namespace Reviews.Endpoints;
public static class MenuItemReviewEndpoints
{
    public static void MapMenuItemReviewEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/reviews");

        group.MapGet("/{id}", async (int id) =>
        {

        });

        group.MapPost("/", async (Object dto) =>
        {

        });
    }
}