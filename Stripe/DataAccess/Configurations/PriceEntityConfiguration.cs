using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stripe.DataAccess.Entities;
using Stripe.DataAccess.Entities.Enums;

namespace Stripe.DataAccess.Configurations;

public class PriceEntityConfiguration: IEntityTypeConfiguration<PriceEntity>
{
    public void Configure(EntityTypeBuilder<PriceEntity> builder)
    {
        builder.HasKey(price => price.Id);

        builder
            .HasOne<SubscriptionPlanEntity>(price => price.SubscriptionPlan)
            .WithMany(subscriptionPlan => subscriptionPlan.Prices)
            .HasForeignKey(subscriptionPlan => subscriptionPlan.SubscriptionPlanId);

        builder.HasData(
            new PriceEntity
            {
                Id = 1,
                SubscriptionPlanId = 1,
                StripePriceId = "price_1SKy0qCLnke0wpIT5p6NYVQw",
                Price = 5,
                Currency = "USD",
                BillingPeriod = PriceBillingPeriod.Monthly
            },
            new PriceEntity
            {
                Id = 2,
                SubscriptionPlanId = 1,
                StripePriceId = "price_1SJDPHCLnke0wpITkJq26ra0",
                Price = 50,
                Currency = "USD",
                BillingPeriod = PriceBillingPeriod.Yearly
            },
            new PriceEntity
            {
                Id = 3,
                SubscriptionPlanId = 2,
                StripePriceId = "price_1SJDObCLnke0wpITy9PHxVxK",
                Price = 10,
                Currency = "USD",
                BillingPeriod = PriceBillingPeriod.Monthly
            },
            new PriceEntity
            {
                Id = 4,
                SubscriptionPlanId = 2,
                StripePriceId = "price_1SKy3hCLnke0wpITi2xo7uBT",
                Price = 100,
                Currency = "USD",
                BillingPeriod = PriceBillingPeriod.Yearly
            },
            new PriceEntity
            {
                Id = 5,
                SubscriptionPlanId = 3,
                StripePriceId = "price_1SJDOtCLnke0wpITTywacmtv",
                Price = 15,
                Currency = "USD",
                BillingPeriod = PriceBillingPeriod.Monthly
            },
            new PriceEntity
            {
                Id = 6,
                SubscriptionPlanId = 3,
                StripePriceId = "price_1SKy34CLnke0wpITpjafca5U",
                Price = 150,
                Currency = "USD",
                BillingPeriod = PriceBillingPeriod.Yearly
            });
    }
}
