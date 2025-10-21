using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stripe.DataAccess.Entities;

namespace Stripe.DataAccess.Configurations;

public class SubscriptionEntityConfiguration : IEntityTypeConfiguration<SubscriptionEntity>
{
    public void Configure(EntityTypeBuilder<SubscriptionEntity> builder)
    {
        builder
            .HasMany<UserEntity>(subscription => subscription.Users)
            .WithMany(user => user.Subscriptions)
            .UsingEntity<UserSubscription>();
            
        builder
            .HasOne<PriceEntity>(subscription => subscription.Price)
            .WithOne(price => price.Subscription)
            .HasForeignKey<SubscriptionEntity>(subscription => subscription.PriceId);
        
        builder.HasData(
            new SubscriptionEntity
            {
                Id = 1,
                Name = "TestSubscriptionMonthly-1",
                PriceId = "price_1SJDObCLnke0wpITy9PHxVxK",
                StripeProductId = "prod_TFiqgpwQsYS69k"
            },
            new SubscriptionEntity
            {
                Id = 2,
                Name = "TestSubscriptionMonthly-2",
                PriceId = "price_1SJDOtCLnke0wpITTywacmtv",
                StripeProductId = "prod_TFiqBLnYKGafcO"
            },
            new SubscriptionEntity
            {
                Id = 3,
                Name = "TestSubscriptionYearly-1",
                PriceId = "price_1SJDPHCLnke0wpITkJq26ra0",
                StripeProductId = "prod_TFiqRZlgyUG3cJ"
            });
    }
}
