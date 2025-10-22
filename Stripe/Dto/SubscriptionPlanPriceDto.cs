using Stripe.DataAccess.Entities.Enums;

namespace Stripe.Dto;

public class SubscriptionPlanPriceDto
{
    public int Id { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; }
    public PriceBillingPeriod BillingPeriod { get; set; }
}
