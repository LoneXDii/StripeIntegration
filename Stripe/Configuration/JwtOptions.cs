namespace Stripe.Configuration;

public class JwtOptions
{
    public string Secret { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int AccessTokenExpireMinutes { get; set; }
    public int RefreshTokenExpireDays { get; set; }
}
