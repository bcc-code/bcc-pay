namespace BccPay.Core.Sample.Contracts.Requests
{
    public class PaymentConfigurationRequest
    {
        public string CountryCode { get; set; }
        public string PaymentType { get; set; }
        public string CurrencyCode { get; set; }
    }
}
