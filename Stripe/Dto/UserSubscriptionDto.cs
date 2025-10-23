using Stripe.DataAccess.Entities.Enums;

namespace Stripe.Dto;

public class UserSubscriptionDto
{
    public string SubscriptionPlanName { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; }
    public PriceBillingPeriod BillingPeriod { get; set; }
    public DateTime StartDateTimeUtc { get; set; }
    public DateTime PeriodEndDateTimeUtc { get; set; }
    public DateTime? EndDateTimeUtc { get; set; }
    public string Status { get; set; }
}
