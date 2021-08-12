using System;
using BccPay.Core.Enums;

namespace BccPay.Core.Domain.Entities
{
    public record CurrencyConversionRecord(
       DateTime? RateUpdated,
       Currencies FromCurrency,
       Currencies ToCurrency,
       decimal Rate,
       decimal MidMarketRate,
       decimal FromAmount,
       decimal ToAmount);
}
