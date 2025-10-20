namespace Stripe.Services.Interfaces;

public interface ISubscriptionService
{
    Task<string> BuySubscriptionAsync(int subscriptionId, CancellationToken cancellationToken);
    Task ProcessWebhookAsync(string eventJson, string signature, CancellationToken cancellationToken);
}