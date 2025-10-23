using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Stripe.Services.Interfaces;

namespace Stripe.Controllers;

[ApiController]
[Route("api/subscription-plans")]
public class SubscriptionController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly IStripeService _stripeService;
    
    public SubscriptionController(
        ISubscriptionService subscriptionService,
        IStripeService stripeService)
    {
        _subscriptionService = subscriptionService;
        _stripeService = stripeService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetSubscriptionPlansAsync(CancellationToken cancellationToken)
    {
        var subscriptionPlans = await _subscriptionService.GetSubscriptionPlansAsync(cancellationToken);
        
        return Ok(subscriptionPlans);
    }
    
    [HttpGet("prices/{priceId:int}/checkout")]
    [Authorize]
    public async Task<IActionResult> GetCheckoutUrlAsync(
        [FromRoute] int priceId,
        CancellationToken cancellationToken)
    {
        var stripeCustomerId = HttpContext.User.FindFirst("StripeId")?.Value;
        
        var checkoutUrl = await _subscriptionService.GetCheckoutUrlAsync(priceId, stripeCustomerId, cancellationToken);
        
        return Redirect(checkoutUrl);
    }

    [HttpGet("management")]
    [Authorize]
    public async Task<IActionResult> GetCustomerPortalUrlAsync(CancellationToken cancellationToken)
    {
        var stripeId = User.FindFirst("StripeId")?.Value;
        
        var billingPortalUrl = await _stripeService.GetCustomerPortalUrlAsync(stripeId, cancellationToken);
        
        return Redirect(billingPortalUrl);
    }
}
