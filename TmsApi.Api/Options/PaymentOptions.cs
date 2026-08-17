using System.ComponentModel.DataAnnotations;
public class PaymentOptions
{
    [Required]
    public required string GatewayUrl { get; init; }
}