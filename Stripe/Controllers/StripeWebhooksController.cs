using Microsoft.AspNetCore.Mvc;
using Stripe.Services.Interfaces;

namespace Stripe.Controllers;

[ApiController]
[Route("api/webhooks/stripe")]
public class StripeWebhooksController : ControllerBase
{
    private readonly IStripeService _stripeService;
    
    public StripeWebhooksController(
        IStripeService stripeService)
    {
        _stripeService = stripeService;
    }
    
    [HttpPost("subscriptions")]
    public async Task<IActionResult> ProcessSubscriptionsWebhookAsync(CancellationToken cancellationToken)
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync(cancellationToken);
        
        var signature = Request.Headers["Stripe-Signature"];
        
        await _stripeService.ProcessSubscriptionsWebhookAsync(json, signature, cancellationToken);
        
        return Ok();
    }
}