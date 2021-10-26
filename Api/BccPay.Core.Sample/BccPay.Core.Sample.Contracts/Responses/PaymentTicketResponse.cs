using System;

namespace BccPay.Core.Sample.Contracts.Responses
{
    public record PaymentTicketResponse(
        decimal BaseCurrencyAmount,
        decimal OtherCurrencyAmount,
        decimal ExchangeRate);
}