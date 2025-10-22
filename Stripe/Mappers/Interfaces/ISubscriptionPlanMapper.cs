using Stripe.DataAccess.Entities;
using Stripe.Dto;

namespace Stripe.Mappers.Interfaces;

public interface ISubscriptionPlanMapper
{
    SubscriptionPlanDto MapEntityToSubscriptionPlanDto(SubscriptionPlanEntity entity);
}
