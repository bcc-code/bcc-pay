using BccPay.Core.Enums;

namespace BccPay.Core.Sample.Contracts.Responses
{
    public record GetCurrencyExchangeByDefinitionResponse(
        Currencies FromCurrency,
        Currencies ToCurrency,
        decimal ExchangeRate);
}