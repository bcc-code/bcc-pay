using System.Threading;
using System.Threading.Tasks;
using BccPay.Core.Enums;

namespace BccPay.Core.Infrastructure.Helpers
{
    public interface ICurrencyService
    {
        Task<(decimal, decimal)> Exchange(Currencies formCurrency, Currencies toCurrency, decimal amount, decimal tax = 0);
        Task UpsertByBaseCurrencyRate(Currencies currency = Currencies.NOK, CancellationToken cancellationToken = default);
    }
}
