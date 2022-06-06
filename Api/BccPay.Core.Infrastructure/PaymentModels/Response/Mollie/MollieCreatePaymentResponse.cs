using System;
using System.Text.Json.Serialization;
using BccPay.Core.Infrastructure.PaymentModels.MollieNodes;

namespace BccPay.Core.Infrastructure.PaymentModels.Response.Mollie;

public class MollieCreatePaymentResponse
{
    public string Resource { get; set; }
    public string Id { get; set; }
    public string Mode { get; set; }
    public DateTime CreatedAt { get; set; }
    public MollieAmount Amount { get; set; }
    public string Description { get; set; }
    public object Method { get; set; }
    public MollieMetadata Metadata { get; set; }
    public string Status { get; set; }
    public bool IsCancelable { get; set; }
    public DateTime ExpiresAt { get; set; }
    public object Details { get; set; }
    public string ProfileId { get; set; }
    public string SequenceType { get; set; }
    public string RedirectUrl { get; set; }
    public string WebhookUrl { get; set; }

    [JsonPropertyName("_links")]
    public MollieLinks Links { get; set; }
}
