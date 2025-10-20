namespace Stripe.Services.Interfaces;

public interface ISubscriptionService
{
    Task<string> BuySubscriptionAsync(int subscriptionId, CancellationToken cancellationToken);
}