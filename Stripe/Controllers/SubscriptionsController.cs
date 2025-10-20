using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<ActionResult> BuySubscriptionAsync(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var paymentUrl = await _subscriptionService.BuySubscriptionAsync(id, cancellationToken);
        
        //return Redirect(paymentUrl);
        return Ok(paymentUrl);
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> ProcessWebhookAsync(CancellationToken cancellationToken)
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync(cancellationToken);

        return Ok();
    }
}