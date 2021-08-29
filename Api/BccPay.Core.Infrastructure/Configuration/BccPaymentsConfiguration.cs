using System.Collections.Generic;
using BccPay.Core.Domain;
using BccPay.Core.Enums;
using static BccPay.Core.Domain.Entities.Country;

namespace BccPay.Core.Infrastructure.Configuration
{
    public class BccPaymentsConfiguration
    {
        public List<BccPaymentConfiguration> PaymentConfigurations { get; set; } = new List<BccPaymentConfiguration>();

        public List<CountryBccPaymentConfigurations> CountryPaymentConfigurations { get; set; } = new List<CountryBccPaymentConfigurations>();
    }

    public class BccPaymentConfiguration
    {
        public string Id { get; init; }

        public PaymentProvider Provider { get; set; }

        public PaymentSettings Settings { get; set; }
    }

    public class CountryBccPaymentConfigurations
    {
        public string CountryCode { get; init; }

        public string Condition { get; set; }

        public ConditionalPaymentConfiguration[] PaymentConfigurations { get; set; }
    }
}
