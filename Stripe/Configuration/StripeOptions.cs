namespace Stripe.Configuration;

public class StripeOptions
{
    public string WebhookSecret { get; set; }
    public string CheckoutSuccessUrl { get; set; }
    public string CheckoutCancelUrl { get; set; }
    public string CustomerPortalReturnUrl { get; set; }
}
