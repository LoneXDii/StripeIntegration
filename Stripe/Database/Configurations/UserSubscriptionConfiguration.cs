using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stripe.Database.Entities;

namespace Stripe.Database.Configurations;

public class UserSubscriptionConfiguration : IEntityTypeConfiguration<UserSubscription>
{
    public void Configure(EntityTypeBuilder<UserSubscription> builder)
    {
        builder
            .HasKey(userSubscription => userSubscription.StripeSubscriptionId);
        
        builder
            .HasOne(userSubscription => userSubscription.Subscription)
            .WithMany()
            .HasForeignKey(userSubscription => userSubscription.SubscriptionId);
        
        builder
            .HasOne(userSubscription => userSubscription.User)
            .WithMany()
            .HasForeignKey(userSubscription => userSubscription.UserId);
    }
}