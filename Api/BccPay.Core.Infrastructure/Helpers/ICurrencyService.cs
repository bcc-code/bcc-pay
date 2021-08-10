using System.Threading;
using System.Threading.Tasks;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;

namespace BccPay.Core.Infrastructure.Helpers
{
    public interface ICurrencyService
    {
        Task<CurrencyConversionRecord> Exchange(string fromCurrency, string toCurrency, decimal amount, decimal exchangeRateMarkup = 0);
        Task<CurrencyConversionRecord> Exchange(Currencies fromCurrency, Currencies toCurrency, decimal amount, decimal exchangeRateMarkup = 0);
        Task UpsertByBaseCurrencyRate(Currencies currency = Currencies.NOK, CancellationToken cancellationToken = default);
    }
}
