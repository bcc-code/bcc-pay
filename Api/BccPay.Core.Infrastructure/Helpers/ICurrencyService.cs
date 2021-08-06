using System.Threading;
using System.Threading.Tasks;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.Dtos;

namespace BccPay.Core.Infrastructure.Helpers
{
    public interface ICurrencyService
    {
        Task<CurrencyExchangeResult> Exchange(Currencies formCurrency, Currencies toCurrency, decimal amount, decimal tax = 0);
        Task UpsertByBaseCurrencyRate(Currencies currency = Currencies.NOK, CancellationToken cancellationToken = default);
    }
}
