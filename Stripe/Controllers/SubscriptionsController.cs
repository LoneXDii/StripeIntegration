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

    public SubscriptionController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    [HttpGet("{id:int}/payment")]
    [Authorize]
    public async Task<ActionResult> GetPaymentUrlAsync(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var paymentUrl = await _subscriptionService.GetPaymentUrlAsync(id, cancellationToken);
        
        return Redirect(paymentUrl);
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
