namespace Stripe.Database.Entities;

public class SubscriptionEntity
{
    public int Id { get; set; }
    public string StripePriceId { get; set; } = null!;
    public string StripeProductId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public double Price { get; set; }
    public ICollection<UserEntity> Users { get; set; }
}