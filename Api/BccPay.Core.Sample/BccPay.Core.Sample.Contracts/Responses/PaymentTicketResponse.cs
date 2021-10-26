using System;

namespace BccPay.Core.Sample.Contracts.Responses
{
    public record PaymentTicketResponse(
        Guid TicketId,
        decimal? BaseCurrencyAmount,
        decimal? OtherCurrencyAmount,
        decimal? ExchangeRate,
        DateTime? LastUpdate);
}