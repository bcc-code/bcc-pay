using System.Threading.Tasks;
using BccPay.Core.Infrastructure.Dtos;
using Refit;

namespace BccPay.Core.Infrastructure.RefitClients;

public interface IFixerClient
{
    [Get("/latest")]
    public Task<FixerCurrencyRateResponse> GetRatesByBaseCurrency([AliasAs("access_key")] string accessKey, [AliasAs("base")] string baseCurrency);
}
