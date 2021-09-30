using BccPay.Core.Enums;

namespace BccPay.Core.Sample.Contracts.Requests
{
    public class PaymentConfigurationRequest
    {
        public string CountryCode { get; set; }
        public string PaymentType { get; set; }
        public Currencies? CurrencyCode { get; set; }
    }
}
