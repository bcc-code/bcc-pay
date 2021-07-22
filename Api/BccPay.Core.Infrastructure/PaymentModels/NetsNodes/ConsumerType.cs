namespace BccPay.Core.Infrastructure.PaymentModels.NetsNodes
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;
    public class ConsumerType
    {
        [JsonPropertyName("supportedTypes")]
        public List<string> SupportedTypes { get; set; }

        [JsonPropertyName("default")]
        public string Default { get; set; }
    }
}