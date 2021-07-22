namespace BccPay.Core.Contracts.Requests.Nodes
{
    public class CheckoutRequest
    {
        public string TermsUrl { get; set; }
        public bool PublicDevice { get; set; }
        public bool Charge { get; set; }
        public string IntegrationType { get; set; }
        public bool MerchantHandlesConsumerData { get; set; }
        public ConsumerTypeRequest ConsumerType { get; set; }
        public string Url { get; set; }
    }
}