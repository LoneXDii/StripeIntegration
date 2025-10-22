using Microsoft.EntityFrameworkCore;
using Stripe.DataAccess;
using Stripe.Dto;
using Stripe.Exceptions;
using Stripe.Mappers.Interfaces;
using Stripe.Services.Interfaces;

namespace Stripe.Services.Implementations;

public class SubscriptionService : ISubscriptionService
{
    private readonly IDbContext _dbContext;
    private readonly IStripeService _stripeService;
    private readonly ISubscriptionPlanMapper _subscriptionPlanMapper;
    
    public SubscriptionService(
        IDbContext dbContext,
        IStripeService stripeService,
        ISubscriptionPlanMapper subscriptionPlanMapper)
    {
        
        _dbContext = dbContext;
        _stripeService = stripeService;
        _subscriptionPlanMapper = subscriptionPlanMapper;
    }

    public async Task<string> GetCheckoutUrlAsync(int subscriptionPriceId, string stripeCustomerId, CancellationToken cancellationToken)
    {
        var isUserAlreadySubscribed = await _dbContext.Users
            .AnyAsync(u => u.StripeId == stripeCustomerId && u.UserSubscription == null, cancellationToken);

        if (isUserAlreadySubscribed)
        {
            throw new BadRequestException("This user already has a subscription.");
        }
        
        var price = await _dbContext.Prices
            .FirstOrDefaultAsync(price => price.Id == subscriptionPriceId, cancellationToken);

        if (price is null)
        {
            throw new NotFoundException($"Price with id: {subscriptionPriceId} does not exist.");
        }
        
        var paymentUrl = await _stripeService.GetCheckoutUrlAsync(
            price.StripePriceId,
            stripeCustomerId,
            cancellationToken);
        
        return paymentUrl;
    }

    public async Task<List<SubscriptionPlanDto>> GetSubscriptionPlansAsync(CancellationToken cancellationToken)
    {
        var subscriptionPlans = await _dbContext.SubscriptionPlans
            .Include(s => s.Prices)
            .ToListAsync(cancellationToken);
        
        return subscriptionPlans
            .Select(subscriptionPlanEntity => _subscriptionPlanMapper.MapEntityToSubscriptionPlanDto(subscriptionPlanEntity))
            .ToList();
    }
}
