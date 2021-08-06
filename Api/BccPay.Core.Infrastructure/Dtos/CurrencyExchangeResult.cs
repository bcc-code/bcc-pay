using BccPay.Core.Enums;

namespace BccPay.Core.Infrastructure.Dtos
{
    public record CurrencyExchangeResult(
       Currencies FromCurrency,
       Currencies ToCurrency,
       decimal Amount,
       decimal Gross,
       decimal Netto,
       decimal Tax);
}
