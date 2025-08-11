using Payments.Contracts.Dtos;
using Payments.Models;
using Shared.Contracts.Interfaces;
using Stripe.Checkout;

namespace Payments.Services.IServices;

public interface IPaymentsService
{
    /// <summary>
    /// This method will ensure we always have customer before checking out and its set to the checkout session
    /// </summary>
    Task<string> CreateCheckoutSessionAsync(CreateCheckoutSessionDto request);
    Task<Session> GetSessionByIdAsync(string sessionId, List<string>? extraExpands = null);
    Task<(string brand, string last4)> GetCardDetailsAsync(string paymentIntentId);
    Task<AppStripeCustomer> CreateStripeCustomerAsync(IUserDetails userDetails, IUserInfo userInfo);
    Task<Payment> CreatePaymentAsync(Payment payment);
}