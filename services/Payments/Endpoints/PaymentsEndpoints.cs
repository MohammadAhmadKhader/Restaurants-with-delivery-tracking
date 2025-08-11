using Payments.Contracts.Dtos;
using Payments.Services.IServices;
using Shared.Auth;

namespace Payments.Endpoints;
public static class PaymentsEndpoints
{
    public static void MapPaymentsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/payments");

        group.MapPost("/checkout-session", async (CreateCheckoutSessionDto dto, IPaymentsService paymentsService) =>
        {
            var sessionUrl = await paymentsService.CreateCheckoutSessionAsync(dto);
            
            return Results.Ok(new { sessionUrl });
        }).RequireAuthenticatedUser();
    }
}