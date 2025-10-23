namespace Stripe.Services.Interfaces;

public interface IStripeService
{
    Task<string> GetCheckoutUrlAsync(string stripePriceId, string stripeCustomerId, CancellationToken cancellationToken);
    Task<string> GetCustomerPortalUrlAsync(string stripeCustomerId, CancellationToken cancellationToken);
    Task ProcessSubscriptionsWebhookAsync(string eventJson, string signature, CancellationToken cancellationToken);
}
