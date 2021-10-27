using System;

namespace BccPay.Core.Infrastructure.Dtos
{
    public record PaymentTicketResponse(
        Guid TicketId,
        decimal? BaseCurrencyAmount,
        decimal? OtherCurrencyAmount,
        decimal? ExchangeRate,
        DateTime? LastUpdate,
        string PaymentDefinition,
        string CountryCode);
}