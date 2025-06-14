namespace Orders.Endpoints;
public static class OrdersEndpoints
{
    public static void MapOrdersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders");

        group.MapGet("/{id}", async (int id) =>
        {

        });

        group.MapPost("/", async (Object dto) =>
        {

        });
    }
}