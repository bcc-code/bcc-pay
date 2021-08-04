using BccPay.Core.Enums;

namespace BccPay.Core.Domain
{
    public class PaymentConfiguration
    {
        public static string GetPaymentConfigurationId(string countryCode) => $"payment-configurations/{countryCode}";

        public string Id => GetPaymentConfigurationId(PaymentConfigurationCode);

        public string CountryCode { get; set; }

        public string PaymentConfigurationCode { get; init; }

        public PaymentProvider Provider { get; init; }

        public PaymentProviderSettings Settings { get; set; }
    }

    public class PaymentProviderSettings
    {
        public PaymentMethod PaymentMethod { get; init; }

        public string Currency { get; init; }
    }
}
