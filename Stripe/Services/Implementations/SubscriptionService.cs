using Microsoft.EntityFrameworkCore;
using Stripe.Database;
using Stripe.Exceptions;
using Stripe.Services.Interfaces;

namespace Stripe.Services.Implementations;

public class SubscriptionService : ISubscriptionService
{
    private readonly AppDbContext _dbContext;
    private readonly IPaymentService _paymentService;
    private readonly HttpContext _httpContext;
    
    public SubscriptionService(
        AppDbContext dbContext,
        IPaymentService paymentService,
        IHttpContextAccessor httpContextAccessor)
    {
        
        _dbContext = dbContext;
        _paymentService = paymentService;
        _httpContext = httpContextAccessor.HttpContext;
    }

    public async Task<string> BuySubscriptionAsync(int subscriptionId, CancellationToken cancellationToken)
    {
        var subscription = await _dbContext.Subscriptions
            .FirstOrDefaultAsync(subscription => subscription.Id == subscriptionId, cancellationToken);

        if (subscription is null)
        {
            throw new NotFoundException($"Subscription with id: {subscriptionId} does not exist.");
        }
        
        var stripeCustomerId = _httpContext?.User?.FindFirst("StripeId")?.Value;

        if (stripeCustomerId is null)
        {
            throw new BadRequestException("User not found.");
        }
        
        var paymentUrl = await _paymentService.GetPaymentUrlAsync(
            subscription.StripePriceId,
            stripeCustomerId,
            cancellationToken);
        
        return paymentUrl;
    }
}