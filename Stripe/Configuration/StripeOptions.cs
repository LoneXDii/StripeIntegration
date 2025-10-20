namespace Stripe.Configuration;

public class StripeOptions
{
    public string SecretKey { get; set; }
    public string PublishableKey { get; set; }
    public string Secret { get; set; }
    public string SessionMode { get; set; }
    public string SuccessUrl { get; set; }
    public string CancelUrl { get; set; }
    public string CustomerPortalUrl { get; set; }
}