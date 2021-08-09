using System;
using System.Collections.Generic;
using BccPay.Core.Enums;

namespace BccPay.Core.Domain.Entities
{
    public class CurrencyRate
    {
        public static string GetCurrencyRateId(Currencies baseCurrency)
             => $"currencyRate/{baseCurrency}";

        public string Id => GetCurrencyRateId(BaseCurrency);

        public Currencies BaseCurrency { get; set; }
        public IDictionary<Currencies, decimal> Rates { get; set; }
        public DateTime ServerUpdate { get; set; }
        public DateTime? FixerServerUpdate { get; set; }

        public void Update(
            IDictionary<Currencies, decimal> rates,
            DateTime? externalUpdate)
        {
            ServerUpdate = DateTime.UtcNow;
            FixerServerUpdate = externalUpdate;

            Rates = rates;
        }
    }
}
