using BccPay.Core.Infrastructure.Dtos;
using Refit;
using System.Threading.Tasks;

namespace BccPay.Core.Infrastructure.RefitClients
{
    public interface IFixerClient
    {
        [Get("/latest")]
        public Task<FixerCurrencyRateResponse> GetLatestCurrencyRate([AliasAs("access_key")] string accessKey, int format);
    }
}
