namespace Stripe.DataAccess.Entities;

public class SubscriptionEntity
{
    public int Id { get; set; }
    public string StripeProductId { get; set; }
    public string Name { get; set; }
    public string PriceId { get; set; }
    public PriceEntity Price { get; set; }
    public ICollection<UserEntity> Users { get; set; }
}
