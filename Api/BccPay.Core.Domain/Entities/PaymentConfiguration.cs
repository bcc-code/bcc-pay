namespace BccPay.Core.Domain.Entities
{
    public class PaymentConfiguration
    {
        public PaymentConditions Conditions { get; set; }
        public string[] PaymentProviderDefinitionIds { get; set; }
    }

    public class PaymentConditions
    {
        public string[] CountryCodes { get; set; }
        public string[] PaymentTypes { get; set; }
        public string[] CurrencyCodes { get; set; }
    }
}
