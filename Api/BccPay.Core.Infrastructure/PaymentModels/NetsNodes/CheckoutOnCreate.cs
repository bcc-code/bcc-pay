namespace BccPay.Core.Infrastructure.PaymentModels.NetsNodes
{
    using System.Text.Json.Serialization;

    public class CheckoutOnCreate
    {
        [JsonPropertyName("termsUrl")]
        public string TermsUrl { get; set; }

        [JsonPropertyName("publicDevice")]
        public bool PublicDevice { get; set; }

        [JsonPropertyName("charge")]
        public bool Charge { get; set; }

        [JsonPropertyName("integrationType")]
        public string IntegrationType { get; set; }

        [JsonPropertyName("merchantHandlesConsumerData")]
        public bool MerchantHandlesConsumerData { get; set; }

        [JsonPropertyName("consumerType")]
        public ConsumerType ConsumerType { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}