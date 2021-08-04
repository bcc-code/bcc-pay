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
        public Currencies From { get; set; }
        public List<Currencies> To { get; set; }
        public decimal Rate { get; set; }
        public DateTime ServerUpdate { get; set; }
        public DateTime? CurrencyExternalUpdate { get; set; }
    }
}
