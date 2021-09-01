using System.Collections.Generic;
using BccPay.Core.Domain;
using BccPay.Core.Enums;

namespace BccPay.Core.Infrastructure.Configuration
{
    public class BccPaymentsConfiguration
    {
        public List<BccPaymentConfiguration> PaymentProviderDefinitions { get; set; } = new List<BccPaymentConfiguration>();

        public List<PaymentConfigurations> PaymentConfigurations { get; set; } = new List<PaymentConfigurations>();
    }

    public class BccPaymentConfiguration
    {
        public string Id { get; init; }

        public PaymentProvider Provider { get; set; }

        public PaymentSettings Settings { get; set; }
    }

    public class PaymentConfigurations
    {
        public Conditions Conditions { get; set; }
        public string[] PaymentProviderDefinitionIds { get; set; }
    }

    public class Conditions
    {
        public string[] CountryCodes { get; set; }
        public string[] PaymentTypes { get; set; }
        public string[] CurrencyCodes { get; set; }
    }
}
