using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stripe.DataAccess.Entities;
using Stripe.DataAccess.Entities.Enums;

namespace Stripe.DataAccess.Configurations;

public class PriceEntityConfiguration: IEntityTypeConfiguration<PriceEntity>
{
    public void Configure(EntityTypeBuilder<PriceEntity> builder)
    {
        builder.HasKey(price => price.StripePriceId);

        builder.HasData(
            new PriceEntity
            {
                StripePriceId = "price_1SJDObCLnke0wpITy9PHxVxK",
                PriceUsd = 10,
                BillingPeriod = PriceBillingPeriod.Monthly
            },
            new PriceEntity
            {
                StripePriceId = "price_1SJDOtCLnke0wpITTywacmtv",
                PriceUsd = 15,
                BillingPeriod = PriceBillingPeriod.Monthly
            },
            new PriceEntity
            {
                StripePriceId = "price_1SJDPHCLnke0wpITkJq26ra0",
                PriceUsd = 50,
                BillingPeriod = PriceBillingPeriod.Yearly
            });
    }
}
