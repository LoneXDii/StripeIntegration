using Microsoft.AspNetCore.Identity;
namespace Stripe.DataAccess.Entities;

public class UserEntity : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? StripeId { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiresAt { get; set; }
    public UserSubscriptionEntity? UserSubscription { get; set; }
}
