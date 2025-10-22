namespace Stripe.Services.Interfaces;

public interface ISubscriptionService
{
    Task<string> GetCheckoutUrlAsync(int subscriptionId, CancellationToken cancellationToken);
    Task ProcessWebhookAsync(string eventJson, string signature, CancellationToken cancellationToken);
}
