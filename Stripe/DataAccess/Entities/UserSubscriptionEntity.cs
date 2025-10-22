using Stripe.DataAccess.Entities.Enums;

namespace Stripe.DataAccess.Entities;

public class UserSubscriptionEntity
{
    public int Id { get; set; }
    public string StripeSubscriptionId { get; set; }
    public string SubscriptionStatus { get; set; }
    public DateTime StartDateTimeUtc { get; set; }
    public DateTime PeriodEndDateTimeUtc { get; set; }
    public DateTime? EndDateTimeUtc { get; set; }
    public string UserId { get; set; }
    public UserEntity? User { get; set; }
    public int PriceId { get; set; }
    public PriceEntity? Price { get; set; }
    public string StripePriceId { get; set; }
}
