namespace Payments.Services.IServices;
public interface IStripeWebhookService
{
    Task ProcessWebhookAsync(string payload, string signature);
}