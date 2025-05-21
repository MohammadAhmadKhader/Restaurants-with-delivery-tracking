public static class RestaurantReviewEndpoints
{
    public static void MapRestaurantReviewEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/reviews");

        group.MapGet("/{id:int}", async (int id) =>
        {
            
        });

        group.MapPost("/", async (Object dto) =>
        {
    
        });
    }
}