namespace BccPay.Core.Infrastructure.PaymentModels.Response.Nets;

public class NetsChargeResponse
{
    public string ChargeId { get; set; }
    public NetsInvoice Invoice { get; set; }
}

public class NetsInvoice
{
    public string InvoiceNumber { get; set; }
}
