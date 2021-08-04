using BccPay.Core.Infrastructure.Dtos;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BccPay.Core.Infrastructure.RefitClients
{
    public interface IFixerClient
    {
        [Get("api/latest")]
        public Task<FixerCurrencyRateResponse> GetLatestCurrencyRate([Query] string baseCurrency, [Query] List<string> symbols);
    }
}
