public static class OrdersEndpoints
{
    public static void MapOrdersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders");

        group.MapGet("/{id:int}", async (int id) =>
        {
            
        });

        group.MapPost("/", async (Object dto) =>
        {
    
        });
    }
}