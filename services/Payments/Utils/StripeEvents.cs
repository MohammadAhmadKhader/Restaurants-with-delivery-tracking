namespace Payments.Utils;

/// <summary>
/// for now we only care about "checkout.session.completed"
/// </summary>
public static class StripeEventTypes
{
    public const string CheckoutSessionCompleted = "checkout.session.completed";
    public const string PaymentIntentSucceeded = "payment_intent.succeeded";
    public const string ChargeUpdated = "charge.updated";
    public const string ChargedSucceeded = "charge.succeeded";
}