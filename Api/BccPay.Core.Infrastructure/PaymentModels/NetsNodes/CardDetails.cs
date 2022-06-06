namespace BccPay.Core.Infrastructure.PaymentModels.NetsNodes;

public class CardDetails
{
    /// <summary>
    /// A masked version of the PAN (Primary Account Number). At maximum, only the first six and last four digits of the account number are displayed.
    /// </summary>
    public string MaskedPan { get; set; }
    public string ExpiryDate { get; set; }
}
