using BccPay.Core.Enums;

namespace BccPay.Core.Sample.Contracts.Requests
{
    public record ExchangeByDefinitionRequest(string DefinitionId, Currencies? FromCurrency, Currencies? ToCurrency, decimal Amount);
}