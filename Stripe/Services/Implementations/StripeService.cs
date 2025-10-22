using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe.Configuration;
using Stripe.DataAccess;
using Stripe.DataAccess.Entities;
using Stripe.Exceptions;
using Stripe.Services.Interfaces;

namespace Stripe.Services.Implementations;

internal class StripeService : IStripeService
{
    private readonly StripeOptions _stripeOptions;
    private readonly Checkout.SessionService _checkoutSessionService;
    private readonly BillingPortal.SessionService _billingPortalSessionService;
    private readonly IDbContext _dbContext;
    
    public StripeService(
        IOptions<StripeOptions> stripeOptions,
        Checkout.SessionService checkoutSessionService,
        BillingPortal.SessionService billingPortalSessionService,
        IDbContext dbContext)
    {
        _stripeOptions = stripeOptions.Value;
        _checkoutSessionService = checkoutSessionService;
        _billingPortalSessionService = billingPortalSessionService;
        _dbContext = dbContext;
    }
    
    public async Task<string> GetCheckoutUrlAsync(
        string stripePriceId,
        string stripeCustomerId,
        CancellationToken cancellationToken)
    {
        var options = new Checkout.SessionCreateOptions
        {
            Mode = "subscription",
            PaymentMethodTypes = ["card"],
            LineItems =
            [
                new Checkout.SessionLineItemOptions
                {
                    Price = stripePriceId,
                    Quantity = 1,
                }
            ],
            Customer = stripeCustomerId,
            SuccessUrl = _stripeOptions.SuccessUrl,
            CancelUrl = _stripeOptions.CancelUrl,
        };
        
        var session = await _checkoutSessionService.CreateAsync(options, cancellationToken: cancellationToken);
        
        return session.Url;
    }

    public async Task<string> GetCustomerPortalUrlAsync(string stripeCustomerId, CancellationToken cancellationToken)
    {
        var isUserAlreadySubscribed = await _dbContext.Users
            .AnyAsync(u => u.StripeId == stripeCustomerId && u.UserSubscription == null, cancellationToken);

        if (!isUserAlreadySubscribed)
        {
            throw new BadRequestException("This user don't have a subscription.");
        }
        
        var option = new BillingPortal.SessionCreateOptions
        {
            Customer = stripeCustomerId,
            ReturnUrl = _stripeOptions.SuccessUrl,
            Configuration = _stripeOptions.CustomerPortalConfigurationId
        };

        var session = await _billingPortalSessionService.CreateAsync(option, cancellationToken: cancellationToken);
        
        return session.Url;
    }
    
    
    public Task ProcessSubscriptionsWebhookAsync(string eventJson, string signature, CancellationToken cancellationToken)
    {
        var stripeEvent = EventUtility.ConstructEvent(eventJson, signature, _stripeOptions.WebhookSecret);

        if (stripeEvent.Data.Object is not Subscription subscription)
        {
            return Task.CompletedTask;
        }

        return stripeEvent.Type switch
        {
            EventTypes.CustomerSubscriptionCreated => ProcessSubscriptionCreationAsync(subscription, cancellationToken),
            EventTypes.CustomerSubscriptionUpdated => ProcessSubscriptionUpdateAsync(subscription, cancellationToken),
            EventTypes.CustomerSubscriptionDeleted => ProcessSubscriptionDeleteAsync(subscription, cancellationToken),
            _ => Task.CompletedTask
        };
    }

    private async Task ProcessSubscriptionCreationAsync(Subscription subscription, CancellationToken cancellationToken)
    {
        var isSubscriptionExists = await _dbContext.UserSubscriptions
            .AnyAsync(s => s.StripeSubscriptionId == subscription.Id, cancellationToken);

        if (isSubscriptionExists)
        {
            return;
        }
        
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

        if (userSubscription is null)
        {
            await ProcessSubscriptionCreationAsync(subscription, cancellationToken);
            return;
        }
        
        var priceId = subscription.Items.Data[0].Price.Id;
        
        var subscriptionPlanAndPriceIds = await _dbContext.Prices
            .Where(p => p.StripePriceId == priceId)
            .Select(p => new
            {
                PriceId = p.Id,
                SubscriptionPlanId = p.SubscriptionPlanId,
            })
            .FirstOrDefaultAsync(cancellationToken);
        
        if (subscriptionPlanAndPriceIds is null)
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
