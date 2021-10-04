using BccPay.Core.Enums;

namespace BccPay.Core.Domain
{
    public class PaymentProviderDefinition
    {
        public static string GetDocumentId(string code) => $"payment-provider-definitions/{code}";

        public string Id => GetDocumentId(PaymentDefinitionCode);

        public string PaymentDefinitionCode { get; set; }

        public PaymentProvider Provider { get; set; }

        public PaymentSetting Settings { get; set; }
    }

    public class PaymentSetting
    {
        public decimal Markup { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public Currencies Currency { get; set; }
    }
}
