namespace Stripe.Services.Interfaces;

public interface IPaymentService
{
    Task<string> GetPaymentUrlAsync(string stripePriceId, string stripeCustomerId, CancellationToken cancellationToken);
}