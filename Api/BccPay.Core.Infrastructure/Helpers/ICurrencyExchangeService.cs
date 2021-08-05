using BccPay.Core.Enums;
using System.Threading;
using System.Threading.Tasks;

namespace BccPay.Core.Infrastructure.Helpers
{
    public interface ICurrencyExchangeService
    {
        Task<(decimal, decimal)> ExchangeCurrency(Currencies formCurrency, Currencies toCurrency, decimal amount, decimal tax = 0);
        Task UpsertCurrencyRate(CancellationToken cancellationToken);
    }
}
