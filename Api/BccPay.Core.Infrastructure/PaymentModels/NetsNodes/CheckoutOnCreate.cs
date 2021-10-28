namespace BccPay.Core.Infrastructure.PaymentModels.NetsNodes
{
    public class CheckoutOnCreate
    {
        public string TermsUrl { get; set; }

        public bool PublicDevice { get; set; }

        public string ReturnUrl { get; set; }
        public bool Charge { get; set; }

        public string IntegrationType { get; set; }

        public bool MerchantHandlesConsumerData { get; set; }

        public ConsumerType ConsumerType { get; set; }

        public string Url { get; set; }
        public ConsumerOnCreate Consumer { get; set; }
    }
}