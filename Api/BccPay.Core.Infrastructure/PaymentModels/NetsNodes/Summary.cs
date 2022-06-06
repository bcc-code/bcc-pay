namespace BccPay.Core.Infrastructure.PaymentModels.NetsNodes;

public class Summary
{
    public int ReservedAmount { get; set; }
    public int ChargedAmount { get; set; }
    public int RefundedAmount { get; set; }
    public int CancelledAmount { get; set; }
}
