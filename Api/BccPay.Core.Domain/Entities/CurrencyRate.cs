using BccPay.Core.Enums;
using System;
using System.Collections.Generic;

namespace BccPay.Core.Domain.Entities
{
    public class CurrencyRate
    {
        public static string GetCurrencyRateId(Guid currencyRateId)
             => $"currencyRate/{currencyRateId}";

        public string Id => GetCurrencyRateId(CurrencyRateId);
        public Guid CurrencyRateId { get; set; }

        public Currencies Base { get; set; }
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
