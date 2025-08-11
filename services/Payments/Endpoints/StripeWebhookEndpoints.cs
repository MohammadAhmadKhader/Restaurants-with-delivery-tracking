using Payments.Services.IServices;
using Shared.Utils;

namespace Payments.Endpoints;

public static class StripeWebhookEndpoints
{
    public static void MapStripeWebhookEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/webhook");
        group.MapPost("", async (HttpContext ctx, IStripeWebhookService stripeWebhookService) =>
        {
            var req = ctx.Request;
            var json = await new StreamReader(req.Body).ReadToEndAsync();
            var stripeSignature = req.Headers["Stripe-Signature"].FirstOrDefault();
            GuardUtils.ThrowIfNullOrEmpty(stripeSignature);

            await stripeWebhookService.ProcessWebhookAsync(json, stripeSignature);
        });
    }
}