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

    public async Task<string> GetCheckoutUrlAsync(int subscriptionPriceId, CancellationToken cancellationToken)
    {
        var price = await _dbContext.Prices
            .FirstOrDefaultAsync(price => price.Id == subscriptionPriceId, cancellationToken);

        if (price is null)
        {
            throw new NotFoundException($"Price with id: {subscriptionPriceId} does not exist.");
        }
        
        var stripeCustomerId = _httpContext?.User?.FindFirst("StripeId")?.Value;

        if (stripeCustomerId is null)
        {
            throw new BadRequestException("User not found.");
        }
        
        var paymentUrl = await _paymentService.GetCheckoutUrlAsync(
            price.StripePriceId,
            stripeCustomerId,
            cancellationToken);
        
        return paymentUrl;
    }

    public Task ProcessWebhookAsync(string eventJson, string signature, CancellationToken cancellationToken)
    {
        var stripeEvent = EventUtility.ConstructEvent(eventJson, signature, _stripeOptions.Secret);

        if (stripeEvent.Data.Object is not Subscription subscription)
        {
            return Task.CompletedTask;
        }

        return stripeEvent.Type switch
        {
            EventTypes.CustomerSubscriptionCreated => ProcessSubscriptionCreationAsync(subscription, cancellationToken),
            EventTypes.CustomerSubscriptionUpdated => ProcessSubscriptionUpdateAsync(subscription, cancellationToken),
            EventTypes.CustomerSubscriptionDeleted => ProcessSubscriptionDeleteAsync(subscription, cancellationToken),
        };
    }

    private async Task ProcessSubscriptionCreationAsync(Subscription subscription, CancellationToken cancellationToken)
    {
        var userId = await _dbContext.Users
            .Where(u => u.StripeId == subscription.CustomerId)
            .Select(u => u.Id)
            .FirstOrDefaultAsync(cancellationToken);
        
        var priceId = subscription.Items.Data[0].Price.Id;
        
        var subscriptionPlanAndPriceIds = await _dbContext.Prices
            .Where(p => p.StripePriceId == priceId)
            .Select(p => new
            {
                PriceId = p.Id,
                SubscriptionPlanId = p.SubscriptionPlanId,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (userId is null || subscriptionPlanAndPriceIds is null)
        {
            return;
        }

        var userSubscription = new UserSubscriptionEntity
        {
            StripeSubscriptionId = subscription.Id,
            SubscriptionStatus = subscription.Status,
            StartDateTimeUtc = subscription.StartDate,
            PeriodEndDateTimeUtc = subscription.CurrentPeriodEnd,
            EndDateTimeUtc = subscription.CancelAt,
            UserId = userId,
            SubscriptionPlanId = subscriptionPlanAndPriceIds.SubscriptionPlanId,
            PriceId = subscriptionPlanAndPriceIds.PriceId
        };

        _dbContext.UserSubscriptions.Add(userSubscription);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task ProcessSubscriptionUpdateAsync(Subscription subscription, CancellationToken cancellationToken)
    {
        var userSubscription = await _dbContext.UserSubscriptions
            .FirstOrDefaultAsync(us => us.StripeSubscriptionId == subscription.Id, cancellationToken);

        var priceId = subscription.Items.Data[0].Price.Id;
        
        var subscriptionPlanAndPriceIds = await _dbContext.Prices
            .Where(p => p.StripePriceId == priceId)
            .Select(p => new
            {
                PriceId = p.Id,
                SubscriptionPlanId = p.SubscriptionPlanId,
            })
            .FirstOrDefaultAsync(cancellationToken);
        
        if (userSubscription is null || subscriptionPlanAndPriceIds is null)
        {
            return;
        }

        userSubscription.SubscriptionStatus = subscription.Status;
        userSubscription.StartDateTimeUtc = subscription.StartDate;
        userSubscription.PeriodEndDateTimeUtc = subscription.CurrentPeriodEnd;
        userSubscription.EndDateTimeUtc = subscription.CancelAt;
        userSubscription.SubscriptionPlanId = subscriptionPlanAndPriceIds.SubscriptionPlanId;
        userSubscription.PriceId = subscriptionPlanAndPriceIds.PriceId;
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task ProcessSubscriptionDeleteAsync(Subscription subscription, CancellationToken cancellationToken)
    {
        var userSubscription = await _dbContext.UserSubscriptions
            .FirstOrDefaultAsync(us => us.StripeSubscriptionId == subscription.Id, cancellationToken);
            
        if (userSubscription is null)
        {
            return;
        }
        
        _dbContext.UserSubscriptions.Remove(userSubscription);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
