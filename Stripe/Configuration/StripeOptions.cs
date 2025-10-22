namespace Stripe.Configuration;

public class StripeOptions
{
    public string Secret { get; set; }
    public string SuccessUrl { get; set; }
    public string CancelUrl { get; set; }
    public string CustomerPortalConfigurationId { get; set; }
}
