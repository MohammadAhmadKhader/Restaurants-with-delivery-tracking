using MassTransit;
using Microsoft.Extensions.Options;
using Payments.Config;
using Payments.Exceptions;
using Payments.Models;
using Payments.Repositories.IRepositories;
using Payments.Services.IServices;
using Payments.Utils;
using Shared.Exceptions;
using Shared.Kafka;
using Shared.Utils;
using Stripe;
using Stripe.Checkout;

using Event = Stripe.Event;

namespace Payments.Services;

public class StripeWebhookService(
    IOptions<StripeConfig> stripeConfigOptions,
    IPaymentsService paymentsService,
    ILogger<StripeWebhookService> logger,
    ITopicProducer<OrderCheckoutCompleted> orderCheckoutCompleetdProducer,
    IAppStripeCustomersRepository appStripeCustomersRepository
    ) : IStripeWebhookService
{
    private readonly StripeConfig _stripeConfig = stripeConfigOptions.Value;
    private readonly IPaymentsService _paymentsService = paymentsService;
    private readonly ILogger<StripeWebhookService> _logger = logger;
    private readonly ITopicProducer<OrderCheckoutCompleted> _orderCheckoutCompletedProducer = orderCheckoutCompleetdProducer;
    private readonly IAppStripeCustomersRepository _appStripeCustomersRepository = appStripeCustomersRepository;

    public async Task ProcessWebhookAsync(string payload, string signature)
    {
        var webhookSecret = _stripeConfig.WebhookSecret;
        var stripeEvent = EventUtility.ConstructEvent(payload, signature, webhookSecret);


        _logger.LogInformation("Received stripe Event Type {@Event}", stripeEvent.Type);
        await (stripeEvent.Type switch
        {
            StripeEventTypes.CheckoutSessionCompleted => HandleCheckoutSessionCompeleted(stripeEvent),
            _ => LogEventName(stripeEvent.Type)
        });
    }

    private async Task HandleCheckoutSessionCompeleted(Event @event)
    {
        if (@event.Data.Object is Session session)
        {
            _logger.LogInformation("Received stripe Session (Completed) {@Session}", session.ToJson());

            var orderId = session.Metadata.GetValueOrDefault("order_id");
            var restaurantId = session.Metadata.GetValueOrDefault("order_id");

            var (expandedSession, cardDetails) = await TaskUtils.RunAsync(
                _paymentsService.GetSessionByIdAsync(session.Id),
                _paymentsService.GetCardDetailsAsync(session.PaymentIntentId)
            );
            if (expandedSession == null)
            {
                _logger.LogError(
                    "An error has occurred during attempt to fetch expanded session on {Event} {Session}",
                    @event.Type,
                    session);
                throw new InternalServerException("Expanded session was not found.");
            }
            _logger.LogInformation("Received stripe Expanded Session (Completed) {@Session}", expandedSession.ToJson());

            var paymentMethod = expandedSession.PaymentMethodTypes.FirstOrDefault();
            var paymentIntentId = expandedSession.PaymentIntent.Id;
            var chargeId = expandedSession.PaymentIntent.LatestCharge.Id;
            var receiptUrl = expandedSession.PaymentIntent.LatestCharge.ReceiptUrl;
            var paymentStatus = expandedSession.PaymentIntent.Status;

            var appCustoemr = await _appStripeCustomersRepository.FindByStripeCustomerIdAsync(expandedSession.CustomerId);
            if (appCustoemr == null)
            {
                throw new InternalServerException($@"
                    {nameof(AppStripeCustomer)} with {nameof(AppStripeCustomer.StripeCustomerId)} '{expandedSession.CustomerId}' was not found during handling {StripeEventTypes.CheckoutSessionCompleted} event."
                );
            }

            var payment = new Payment()
            {
                Amount = (int)session.AmountTotal!,
                CardBrand = cardDetails.brand,
                CardLast4 = cardDetails.last4,
                ChargeId = chargeId,
                OrderId = Guid.Parse(orderId!),
                PaymentIntentId = paymentIntentId,
                PaymentMethod = paymentMethod!,
                ReceiptUrl = receiptUrl,
                Currency = expandedSession.Currency,
                Status = paymentStatus,
                CreatedAt = DateTime.UtcNow,
                CustomerId = appCustoemr.Id,
            };

            _logger.LogInformation("Session Checkout completed payemnt data {@Payment}.", payment);
            await _paymentsService.CreatePaymentAsync(payment);
            await _orderCheckoutCompletedProducer.Produce(new OrderCheckoutCompleted(payment.OrderId, appCustoemr.RestaurantId, appCustoemr.UserId));
        }
        else
        {
            throw new AppStripeException($"Invalid event type received at {nameof(HandleCheckoutSessionCompeleted)} received '{@event.Type}'");
        }
    }

    public Task LogEventName(string eventName)
    {
        _logger.LogWarning("Received Unhandled Event {@UnhandledEvent}", eventName);
        return Task.CompletedTask;
    }
}