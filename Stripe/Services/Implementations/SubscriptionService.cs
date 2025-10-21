using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe.Configuration;
using Stripe.DataAccess;
using Stripe.DataAccess.Entities;
using Stripe.DataAccess.Entities.Enums;
using Stripe.Exceptions;
using Stripe.Services.Interfaces;

namespace Stripe.Services.Implementations;

public class SubscriptionService : ISubscriptionService
{
    private readonly AppDbContext _dbContext;
    private readonly IPaymentService _paymentService;
    private readonly HttpContext _httpContext;
    private readonly StripeOptions _stripeOptions;
    
    public SubscriptionService(
        AppDbContext dbContext,
        IPaymentService paymentService,
        IHttpContextAccessor httpContextAccessor,
        IOptions<StripeOptions> stripeOptions)
    {
        
        _dbContext = dbContext;
        _paymentService = paymentService;
        _httpContext = httpContextAccessor.HttpContext;
        _stripeOptions = stripeOptions.Value;
    }

    public async Task<string> GetPaymentUrlAsync(int subscriptionId, CancellationToken cancellationToken)
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
            subscription.PriceId,
            stripeCustomerId,
            cancellationToken);
        
        return paymentUrl;
    }

    public Task ProcessWebhookAsync(string eventJson, string signature, CancellationToken cancellationToken)
    {
        var stripeEvent = EventUtility.ConstructEvent(eventJson, signature, _stripeOptions.Secret);

        if (stripeEvent.Data.Object is not Subscription subscription)
        {
            throw new BadRequestException("Invalid event");
        }

        return stripeEvent.Type switch
        {
            EventTypes.CustomerSubscriptionCreated => ProcessSubscriptionCreationAsync(subscription, cancellationToken),
            EventTypes.CustomerSubscriptionUpdated => ProcessSubscriptionUpdateAsync(subscription, cancellationToken),
            EventTypes.CustomerSubscriptionDeleted => ProcessSubscriptionDeleteAsync(subscription, cancellationToken),
            _ => throw new BadRequestException("Invalid event type")
        };
    }

    private async Task ProcessSubscriptionCreationAsync(Subscription subscription, CancellationToken cancellationToken)
    {
        var customerId = subscription.CustomerId;
        var productId = subscription.Items.Data[0].Plan.ProductId;
        var stripeSubscriptionId = subscription.Id;
        
        var userId = await _dbContext.Users
            .Where(u => u.StripeId == customerId)
            .Select(u => u.Id)
            .FirstOrDefaultAsync(cancellationToken);
            
        var subscriptionId = await _dbContext.Subscriptions
            .Where(s => s.StripeProductId == productId)
            .Select(s => s.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (userId is null || subscriptionId == 0)
        {
            throw new NotFoundException($"Specified user or subscription does not exist.");
        }
        
        var userSubscription = new UserSubscription
        {
            UserId = userId,
            SubscriptionId = subscriptionId,
            StripeSubscriptionId = stripeSubscriptionId,
            SubscriptionStatus = SubscriptionStatus.Active,
        };
            
        _dbContext.UserSubscriptions.Add(userSubscription);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task ProcessSubscriptionUpdateAsync(Subscription subscription, CancellationToken cancellationToken)
    {
        var userSubscription = await _dbContext.UserSubscriptions
            .FirstOrDefaultAsync(us => us.StripeSubscriptionId == subscription.Id, cancellationToken);

        if (userSubscription is null)
        {
            throw new NotFoundException($"Subscription with id: {subscription.Id} does not exist.");
        }
        
        userSubscription.SubscriptionStatus = subscription.CancelAt is null 
            ? SubscriptionStatus.Active 
            : SubscriptionStatus.Disabled;
            
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task ProcessSubscriptionDeleteAsync(Subscription subscription, CancellationToken cancellationToken)
    {
        var userSubscription = await _dbContext.UserSubscriptions
            .FirstOrDefaultAsync(us => us.StripeSubscriptionId == subscription.Id, cancellationToken);
            
        if (userSubscription is null)
        {
            throw new NotFoundException($"Subscription with id: {subscription.Id} does not exist.");
        }
        
        _dbContext.UserSubscriptions.Remove(userSubscription);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
