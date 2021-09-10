namespace BccPay.Core.Sample.Contracts.Requests
{
    public class CreatePaymentRequest
    {
        public string PayerId { get; set; }
        public string CurrencyCode { get; set; }
        public string CountryCode { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public SamplePaymentDetails PaymentDetails { get; set; }
    }
}
