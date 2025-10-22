using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stripe.DataAccess.Entities;

namespace Stripe.DataAccess.Configurations;

public class UserSubscriptionConfiguration : IEntityTypeConfiguration<UserSubscriptionEntity>
{
    public void Configure(EntityTypeBuilder<UserSubscriptionEntity> builder)
    {
        builder.HasKey(userSubscription => userSubscription.Id);
        
        builder
            .HasOne(userSubscription => userSubscription.SubscriptionPlan)
            .WithMany(subscriptionPlan => subscriptionPlan.UserSubscriptions)
            .HasForeignKey(userSubscription => userSubscription.SubscriptionPlanId);
        
        builder
            .HasOne(userSubscription => userSubscription.User)
            .WithOne(user => user.UserSubscription)
            .HasForeignKey<UserSubscriptionEntity>(userSubscription => userSubscription.UserId);

        builder
            .HasOne(userSubscription => userSubscription.Price)
            .WithMany()
            .HasForeignKey(userSubscription => userSubscription.PriceId);
    }
}
