namespace BccPay.Core.Infrastructure.PaymentModels.NetsNodes
{
    using System.Text.Json.Serialization;

        public class Webhook
        {
            [JsonPropertyName("eventName")]
            public string EventName { get; set; }

            [JsonPropertyName("url")]
            public string Url { get; set; }

            [JsonPropertyName("authorization")]
            public string Authorization { get; set; }
    }
}