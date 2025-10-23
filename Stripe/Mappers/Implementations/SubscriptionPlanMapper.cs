using Stripe.DataAccess.Entities;
using Stripe.Dto;
using Stripe.Mappers.Interfaces;

namespace Stripe.Mappers.Implementations;

public class SubscriptionPlanMapper : ISubscriptionPlanMapper
{
    public SubscriptionPlanDto MapEntityToSubscriptionPlanDto(SubscriptionPlanEntity entity)
    {
        return new SubscriptionPlanDto()
        {
            Id = entity.Id,
            Name = entity.Name,
            Prices = entity.Prices
                .Select(p => new SubscriptionPlanPriceDto()
                {
                    Id = p.Id,
                    Price = p.Price,
                    Currency = p.Currency,
                    BillingPeriod = p.BillingPeriod
                })
                .ToList()
        };
    }
}
