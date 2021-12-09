using System;
using BccPay.Core.Enums;

namespace BccPay.Core.Infrastructure.Dtos
{
    public record PaymentTicketResponse(
        Guid TicketId,
        Currencies BaseCurrency,
        Currencies OtherCurrency,
        decimal? BaseCurrencyAmount,
        decimal? OtherCurrencyAmount,
        decimal? ExchangeRate,
        decimal? OppositeExchangeRate,
        DateTime? LastUpdate,
        string PaymentDefinition,
        string CountryCode,
        decimal? MinimumAmount,
        decimal? MaximumAmount);
}