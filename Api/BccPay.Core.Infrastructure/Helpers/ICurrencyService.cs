using BccPay.Core.Enums;
using System.Threading;
using System.Threading.Tasks;

namespace BccPay.Core.Infrastructure.Helpers
{
    public interface ICurrencyService
    {
        Task<(decimal, decimal)> Exchange(Currencies formCurrency, Currencies toCurrency, decimal amount, decimal tax = 0);
        Task UpsertCurrencyRate(CancellationToken cancellationToken);
    }
}
