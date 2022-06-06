namespace BccPay.Core.Infrastructure.PaymentModels.MollieNodes;

public class MollieAmount
{
    /// <summary>
    /// https://help.mollie.com/hc/en-us/articles/115000667365
    /// </summary>
    public string Currency { get; set; }
    public string Value { get; set; }
}
