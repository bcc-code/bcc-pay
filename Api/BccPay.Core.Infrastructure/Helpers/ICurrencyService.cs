using System.Threading;
using System.Threading.Tasks;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.Dtos;

namespace BccPay.Core.Infrastructure.Helpers
{
    public interface ICurrencyService
    {
        Task<CurrencyExchangeResult> Exchange(Currencies fromCurrency, Currencies toCurrency, decimal amount, decimal exchangeRateMarkup = 0);
        Task UpsertByBaseCurrencyRate(Currencies currency = Currencies.NOK, CancellationToken cancellationToken = default);
    }
}
