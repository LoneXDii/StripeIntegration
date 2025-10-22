namespace Stripe.DataAccess.Entities;

public class SubscriptionPlanEntity
{
    public int Id { get; set; }
    public string StripeProductId { get; set; }
    public string Name { get; set; }
    public ICollection<PriceEntity> Prices { get; set; }
    public ICollection<UserSubscriptionEntity> UserSubscriptions { get; set; }
}
