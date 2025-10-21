using Microsoft.Extensions.Options;
using Stripe.Checkout;
using Stripe.Configuration;
using Stripe.Services.Interfaces;

namespace Stripe.Services.Implementations;

internal class PaymentService : IPaymentService
{
    private readonly StripeOptions _stripeOptions;
    private readonly SessionService _sessionService;

    public PaymentService(
        IOptions<StripeOptions> stripeOptions,
        SessionService sessionService)
    {
        _stripeOptions = stripeOptions.Value;
        _sessionService = sessionService;
    }
    
    public async Task<string> GetPaymentUrlAsync(
        string stripePriceId,
        string stripeCustomerId,
        CancellationToken cancellationToken)
    {
        var options = new SessionCreateOptions
        {
            Mode = "subscription",
            PaymentMethodTypes = ["card"],
            LineItems =
            [
                new SessionLineItemOptions
                {
                    Price = stripePriceId,
                    Quantity = 1,
                }
            ],
            Customer = stripeCustomerId,
            SuccessUrl = _stripeOptions.SuccessUrl,
            CancelUrl = _stripeOptions.CancelUrl,
        };
        
        var session = await _sessionService.CreateAsync(options, cancellationToken:cancellationToken);
        
        return session.Url;
    }
}
