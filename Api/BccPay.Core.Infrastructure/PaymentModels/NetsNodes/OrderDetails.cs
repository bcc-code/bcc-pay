namespace BccPay.Core.Infrastructure.PaymentModels.NetsNodes;

public class OrderDetails
{
    public int Amount { get; set; }
    public string Currency { get; set; }
    public string Reference { get; set; }
}
