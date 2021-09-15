using System.Collections.Generic;
using BccPay.Core.Enums;

namespace BccPay.Core.Infrastructure.Configuration
{
    public class BccPaymentsConfiguration
    {
        public List<PaymentProviderDefinitions> PaymentProviderDefinitions { get; set; } = new List<PaymentProviderDefinitions>();

        public List<PaymentConfigurations> PaymentConfigurations { get; set; } = new List<PaymentConfigurations>();
    }

    public class PaymentProviderDefinitions
    {
        public string Id { get; init; }

        public PaymentProvider Provider { get; set; }

        public PaymentSettings Settings { get; set; }
    }

    public class PaymentSettings
    {
        public PaymentMethod PaymentMethod { get; init; }

        public Currencies Currency { get; init; }
    }

    public class PaymentConfigurations
    {
        public string CountryCode { get; set; }
        public Conditions Conditions { get; set; }
        public string[] PaymentProviderDefinitionIds { get; set; }
    }

    public class Conditions
    {
        public string[] PaymentTypes { get; set; }
        public string[] CurrencyCodes { get; set; }
    }
}
