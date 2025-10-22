using Stripe.Dto;

namespace Stripe.Services.Interfaces;

public interface ISubscriptionService
{
    Task<string> GetCheckoutUrlAsync(int subscriptionId, string stripeCustomerId, CancellationToken cancellationToken);
    Task<List<SubscriptionPlanDto>> GetSubscriptionPlansAsync(CancellationToken cancellationToken);
}
