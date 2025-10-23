using Stripe.DataAccess.Entities.Enums;

namespace Stripe.DataAccess.Entities;

public class PriceEntity
{
    public int Id { get; set; }
    public string StripePriceId { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; }
    public PriceBillingPeriod BillingPeriod { get; set; }
    public int SubscriptionPlanId { get; set; }
    public SubscriptionPlanEntity? SubscriptionPlan { get; set; }
}
