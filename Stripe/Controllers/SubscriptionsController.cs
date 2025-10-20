using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe.Configuration;
using Stripe.Services.Interfaces;

namespace Stripe.Controllers;

[ApiController]
[Route("/subscriptions")]
public class SubscriptionController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly string _customerPortalUrl;

    public SubscriptionController(
        IOptions<StripeOptions> stripeOptions,
        ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
        _customerPortalUrl = stripeOptions.Value.CustomerPortalUrl;
    }

    [HttpGet("{id:int}/payment")]
    [Authorize]
    public async Task<ActionResult> BuySubscriptionAsync(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var paymentUrl = await _subscriptionService.BuySubscriptionAsync(id, cancellationToken);
        
        return Redirect(paymentUrl);
    }

    [HttpGet("management")]
    [Authorize]
    public IActionResult ManageSubscriptions()
    {
        var email = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
        var url = $"{_customerPortalUrl}?prefilled_email={email}";
        
        return Redirect(url);
    }
    
    [HttpPost("webhook")]
    public async Task<IActionResult> ProcessWebhookAsync(CancellationToken cancellationToken)
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync(cancellationToken);
        
        var signature = Request.Headers["Stripe-Signature"];
        
        await _subscriptionService.ProcessWebhookAsync(json, signature, cancellationToken);
        
        return Ok();
    }
}