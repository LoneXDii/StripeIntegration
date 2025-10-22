using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe.Configuration;
using Stripe.Services.Interfaces;

namespace Stripe.Controllers;

[ApiController]
[Route("subscriptions")]
public class SubscriptionController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly IPaymentService _paymentService;
    
    public SubscriptionController(
        ISubscriptionService subscriptionService,
        IPaymentService paymentService)
    {
        _subscriptionService = subscriptionService;
        _paymentService = paymentService;
    }

    [HttpGet("{id:int}/checkout")]
    [Authorize]
    public async Task<IActionResult> GetCheckoutUrlAsync(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var checkoutUrl = await _subscriptionService.GetCheckoutUrlAsync(id, cancellationToken);
        
        return Ok(checkoutUrl);
        //return Redirect(checkoutUrl);
    }

    [HttpGet("management")]
    [Authorize]
    public async Task<IActionResult> GetCustomerPortalUrlAsync(CancellationToken cancellationToken)
    {
        var stripeId = User.FindFirst("StripeId")?.Value;
        
        var billingPortalUrl = await _paymentService.GetCustomerPortalUrlAsync(stripeId, cancellationToken);
        
        return Ok(billingPortalUrl);
        //return Redirect(billingPortalUrl);
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
