namespace BccPay.Core.Infrastructure.PaymentModels.NetsNodes
{
    using System.Text.Json.Serialization;

    public class CheckoutOnCreate
    {
        public string TermsUrl { get; set; }

        public bool PublicDevice { get; set; }

        public bool Charge { get; set; }

        public string IntegrationType { get; set; }

        public bool MerchantHandlesConsumerData { get; set; }

        public ConsumerType ConsumerType { get; set; }

        public string Url { get; set; }
    }
}