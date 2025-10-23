using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stripe.DataAccess.Entities;

namespace Stripe.DataAccess.Configurations;

public class SubscriptionPlanEntityConfiguration : IEntityTypeConfiguration<SubscriptionPlanEntity>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlanEntity> builder)
    {
        builder.HasData(
            new SubscriptionPlanEntity
            {
                Id = 1,
                Name = "Base",
                StripeProductId = "prod_TFiqRZlgyUG3cJ"
            },
            new SubscriptionPlanEntity
            {
                Id = 2,
                Name = "Premium",
                StripeProductId = "prod_TFiqgpwQsYS69k"
            },
            new SubscriptionPlanEntity
            {
                Id = 3,
                Name = "Ultra",
                StripeProductId = "prod_TFiqBLnYKGafcO"
            });
    }
}
