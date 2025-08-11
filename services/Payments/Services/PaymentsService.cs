using Auth.Contracts.Clients;
using Microsoft.Extensions.Options;
using Orders.Contracts.Clients;
using Payments.Config;
using Payments.Contracts.Dtos;
using Payments.Data;
using Payments.Models;
using Payments.Repositories.IRepositories;
using Payments.Services.IServices;
using Restaurants.Contracts.Dtos.MenuItems;
using Shared.Common;
using Shared.Contracts.Interfaces;
using Shared.Data.Patterns.UnitOfWork;
using Stripe;
using Stripe.Checkout;

namespace Payments.Services;

public class PaymentsService : IPaymentsService
{
    private readonly StripeConfig _stripeConfig;
    private readonly ILogger<PaymentsService> _logger;
    private readonly IAppStripeCustomersRepository _stripeCustomersRepository;
    private readonly IAuthServiceClient _authServiceClient;
    private readonly IOrdersServiceClient _ordersServiceClient;
    private readonly IAuthProvider _authProvider;
    private readonly IResourceBatchRetriever<int, MenuItemViewDto> _resourceBatchRetriever;
    private readonly IUnitOfWork<AppDbContext> _unitOfWork;
    private readonly IPaymentsRepository _paymentsRepository;

    public PaymentsService(
        IAppStripeCustomersRepository stripeCustomersRepository,
        ILogger<PaymentsService> logger,
        IOptions<StripeConfig> stripeOptions,
        IAuthProvider authProvider,
        IResourceBatchRetriever<int, MenuItemViewDto> resourceBatchRetriever,
        IAuthServiceClient authServiceClient,
        IOrdersServiceClient ordersServiceClient,
        IUnitOfWork<AppDbContext> unitOfWork,
        IPaymentsRepository paymentsRepository)
    {
        _stripeCustomersRepository = stripeCustomersRepository;
        _stripeConfig = stripeOptions.Value;
        _authServiceClient = authServiceClient;
        _authProvider = authProvider;
        _logger = logger;
        _resourceBatchRetriever = resourceBatchRetriever;
        _ordersServiceClient = ordersServiceClient;
        _unitOfWork = unitOfWork;
        _paymentsRepository = paymentsRepository;

        StripeConfiguration.ApiKey = _stripeConfig.Secret;
    }
    public async Task<string> CreateCheckoutSessionAsync(CreateCheckoutSessionDto dto)
    {
        var userId = _authProvider.UserInfo.UserId;
        var stripeCustomer = await _stripeCustomersRepository.FindByUserIdAsync(userId);
        if (stripeCustomer == null)
        {
            var userDetails = await _authServiceClient.GetUserDetailsAsync();
            stripeCustomer = await CreateStripeCustomerAsync(userDetails, _authProvider.UserInfo);
        }

        var order = (await _ordersServiceClient.GetOrderByIdWihtItemsAsync(dto.OrderId)).Order;
        if (order.CustomerId != userId)
        {
            throw new UnauthorizedAccessException();
        }

        var orderIdStr = order.Id.ToString();
        var requestedIds = order.Items.Select(oi => oi.MenuItemId);
        var menuItemsById = await _resourceBatchRetriever.ValidateAndRetrieveAsDictAsync(requestedIds, "item");

        List<SessionLineItemOptions> lineItems = [];
        foreach (var orderItem in order.Items)
        {
            lineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = menuItemsById[orderItem.MenuItemId].Name,
                        Description = menuItemsById[orderItem.MenuItemId].Description,
                        Images = [menuItemsById[orderItem.MenuItemId].ImageUrl],
                    },
                    UnitAmount = (int)(menuItemsById[orderItem.MenuItemId].Price * 100),
                },
                Quantity = orderItem.Quantity,
            });
        }

        var restaurantIdStr = _authProvider.UserInfo.RestaurantId.ToString();
        // we only allowed to specify the email or customer id but not both
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = ["card"],
            LineItems = lineItems,
            Mode = "payment",
            Currency = "usd",
            SuccessUrl = _stripeConfig.SuccessUrl,
            CancelUrl = _stripeConfig.CancelUrl,
            Customer = stripeCustomer.StripeCustomerId,
            Metadata = new()
            {
                { "order_id", orderIdStr },
                { "restaurantId", restaurantIdStr}
            },
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options);
        _logger.LogInformation("Created sessino {@Session}", session.ToJson());

        return session.Url;
    }

    public async Task<Session> GetSessionByIdAsync(string sessionId, List<string>? extraExpands = null)
    {
        List<string> mergedExpands = [
            "payment_intent",
            "payment_intent.latest_charge"
        ];
        if (extraExpands != null)
        {
            mergedExpands.AddRange(extraExpands);
        }

        var options = new SessionGetOptions()
        {
            Expand = mergedExpands,
        };

        var service = new SessionService();
        return await service.GetAsync(sessionId, options);
    }

    public async Task<(string brand, string last4)> GetCardDetailsAsync(string paymentIntentId)
    {
        var paymentIntentService = new PaymentIntentService();
        var paymentIntent = await paymentIntentService.GetAsync(paymentIntentId, new PaymentIntentGetOptions
        {
            Expand = ["latest_charge"]
        });
        var charge = paymentIntent.LatestCharge;

        var paymentMethodService = new PaymentMethodService();
        var paymentMethod = await paymentMethodService.GetAsync(charge.PaymentMethod);

        return (paymentMethod.Card.Brand, paymentMethod.Card.Last4);
    }

    public async Task<AppStripeCustomer> CreateStripeCustomerAsync(IUserDetails userDetails, IUserInfo userInfo)
    {
        if (!userInfo.RestaurantId.HasValue)
        {
            throw new InvalidOperationException("Customer must belong to a restaurant.");
        }

        var metadata = new Dictionary<string, string>
        {
            ["user_id"] = userDetails.UserId.ToString(),
            ["restaurant_id"] = userInfo.RestaurantId.Value.ToString()
        };

        var options = new CustomerCreateOptions
        {
            Name = $"{userDetails.FirstName} {userDetails.LastName}",
            Email = userDetails.Email,
            Metadata = metadata
        };

        var service = new CustomerService();
        var stripeCustomer = await service.CreateAsync(options);

        var customer = new AppStripeCustomer
        {
            UserId = userDetails.UserId,
            RestaurantId = userInfo.RestaurantId.Value,
            StripeCustomerId = stripeCustomer.Id
        };

        await _stripeCustomersRepository.CreateAsync(customer);
        await _unitOfWork.SaveChangesAsync();

        return customer;
    }

    public async Task<Payment> CreatePaymentAsync(Payment payment)
    {
        var result = await _paymentsRepository.CreateAsync(payment);
        await _unitOfWork.SaveChangesAsync();

        return result;
    }
}