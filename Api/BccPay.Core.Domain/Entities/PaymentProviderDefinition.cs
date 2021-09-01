using BccPay.Core.Enums;

namespace BccPay.Core.Domain
{
    public class PaymentProviderDefinition
    {
        public static string GetDocumentId(string code) => $"payment-provider-definition/{code}";

        public string Id => GetDocumentId(PaymentConfigurationCode);

        public string PaymentConfigurationCode { get; init; }

        public PaymentProvider Provider { get; set; }

        public PaymentSettings Settings { get; set; }
    }

    public class PaymentSettings
    {
        public PaymentMethod PaymentMethod { get; init; }

        public Currency Currency { get; init; }
    }
}
