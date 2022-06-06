using System;
using System.Text.Json.Serialization;

namespace BccPay.Core.Infrastructure.PaymentModels.Webhooks;

public class NetsWebhook
{
    public string Event { get; set; }
    public Data Data { get; set; }
}

public class Data
{
    public string PaymentId { get; set; }
    public OrderWebhook Order { get; set; }
}

public class OrderWebhook
{
    public Amount Amount { get; set; }
}

public class Amount
{
    [JsonPropertyName("amount")]
    public int TotalAmount { get; set; }
    public string Currency { get; set; }
}
