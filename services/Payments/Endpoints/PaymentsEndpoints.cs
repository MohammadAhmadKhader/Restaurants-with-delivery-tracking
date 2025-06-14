namespace Payments.Endpoints;
public static class PaymentsEndpoints
{
    public static void MapPaymentsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/payments");

        group.MapGet("/{id}", async (int id) =>
        {

        });

        group.MapPost("/", async (Object dto) =>
        {

        });
    }
}