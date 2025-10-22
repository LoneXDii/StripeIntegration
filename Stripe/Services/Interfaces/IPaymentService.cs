namespace Stripe.Services.Interfaces;

public interface IPaymentService
{
    Task<string> GetCheckoutUrlAsync(string stripePriceId, string stripeCustomerId, CancellationToken cancellationToken);
    Task<string> GetCustomerPortalUrlAsync(string stripeCustomerId, CancellationToken cancellationToken);
}
