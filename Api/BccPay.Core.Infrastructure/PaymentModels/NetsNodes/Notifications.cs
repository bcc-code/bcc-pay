namespace BccPay.Core.Infrastructure.PaymentModels.NetsNodes
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class Notifications
        {
            [JsonPropertyName("webhooks")]
            public List<Webhook> Webhooks { get; set; }
        }
}