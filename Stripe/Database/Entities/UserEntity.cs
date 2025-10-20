using Microsoft.AspNetCore.Identity;

namespace Stripe.Database.Entities;

public class UserEntity : IdentityUser
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? StripeId { get; set; }
    public ICollection<SubscriptionEntity> Subscriptions { get; set; }
}