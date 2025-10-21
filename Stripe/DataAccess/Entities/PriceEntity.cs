using Stripe.DataAccess.Entities.Enums;

namespace Stripe.DataAccess.Entities;

public class PriceEntity
{
    public string StripePriceId { get; set; }
    public decimal PriceUsd { get; set; }
    public PriceBillingPeriod BillingPeriod { get; set; }
    public SubscriptionEntity Subscription { get; set; }
}
