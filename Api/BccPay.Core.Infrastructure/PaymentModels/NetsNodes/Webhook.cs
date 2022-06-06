namespace BccPay.Core.Infrastructure.PaymentModels.NetsNodes;

public class Webhook
{
    public string EventName { get; set; }

    public string Url { get; set; }

    public string Authorization { get; set; }
}
