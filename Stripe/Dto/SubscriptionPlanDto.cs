namespace Stripe.Dto;

public class SubscriptionPlanDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<SubscriptionPlanPriceDto> Prices { get; set; }
}
