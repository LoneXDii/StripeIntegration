using Stripe.DataAccess.Entities.Enums;

namespace Stripe.DataAccess.Entities;

public class UserSubscription
{
    public string StripeSubscriptionId { get; set; }
    public string UserId { get; set; }
    public int SubscriptionId { get; set; }
    public SubscriptionStatus SubscriptionStatus { get; set; }
    
    public UserEntity User { get; set; }
    public SubscriptionEntity Subscription { get; set; }
}
