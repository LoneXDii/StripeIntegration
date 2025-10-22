using Microsoft.Extensions.Options;
using Stripe.Configuration;
using Stripe.Services.Interfaces;

namespace Stripe.Services.Implementations;

internal class PaymentService : IPaymentService
{
    private readonly StripeOptions _stripeOptions;
    private readonly Checkout.SessionService _checkoutSessionService;
    private readonly BillingPortal.SessionService _billingPortalSessionService;
    public PaymentService(
        IOptions<StripeOptions> stripeOptions,
        Checkout.SessionService checkoutSessionService,
        BillingPortal.SessionService billingPortalSessionService)
    {
        _stripeOptions = stripeOptions.Value;
        _checkoutSessionService = checkoutSessionService;
        _billingPortalSessionService = billingPortalSessionService;
    }
    
    public async Task<string> GetCheckoutUrlAsync(
        string stripePriceId,
        string stripeCustomerId,
        CancellationToken cancellationToken)
    {
        var options = new Checkout.SessionCreateOptions
        {
            Mode = "subscription",
            PaymentMethodTypes = ["card"],
            LineItems =
            [
                new Checkout.SessionLineItemOptions
                {
                    Price = stripePriceId,
                    Quantity = 1,
                }
            ],
            Customer = stripeCustomerId,
            SuccessUrl = _stripeOptions.SuccessUrl,
            CancelUrl = _stripeOptions.CancelUrl,
        };
        
        var session = await _checkoutSessionService.CreateAsync(options, cancellationToken: cancellationToken);
        
        return session.Url;
    }

    public async Task<string> GetCustomerPortalUrlAsync(string stripeCustomerId, CancellationToken cancellationToken)
    {
        var option = new BillingPortal.SessionCreateOptions
        {
            Customer = stripeCustomerId,
            ReturnUrl = _stripeOptions.SuccessUrl,
            Configuration = _stripeOptions.CustomerPortalConfigurationId
        };

        var session = await _billingPortalSessionService.CreateAsync(option, cancellationToken: cancellationToken);
        
        return session.Url;
    }
}
