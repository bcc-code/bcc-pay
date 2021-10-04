using BccPay.Core.Enums;

namespace BccPay.Core.Sample.Contracts.Requests
{
    public record ExchangeWithConfigurationsRequestModel(
            decimal Amount,
            Currencies? FromCurrency,
            Currencies? ToCurrency,
            string CountryCode,
            string PaymentType);
}
