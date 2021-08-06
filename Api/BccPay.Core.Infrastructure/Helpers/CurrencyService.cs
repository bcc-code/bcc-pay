using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.Exceptions;
using BccPay.Core.Infrastructure.RefitClients;
using BccPay.Core.Shared.Converters;
using Microsoft.Extensions.Configuration;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BccPay.Core.Infrastructure.Helpers
{
    public class CurrencyService : ICurrencyService
    {
        private readonly IAsyncDocumentSession _documentSession;
        private readonly IFixerClient _fixerClient;
        private readonly IConfiguration _configuration;

        public CurrencyService(IAsyncDocumentSession documentSession,
            IFixerClient fixerClient,
            IConfiguration configuration)
        {
            _documentSession = documentSession
                ?? throw new ArgumentNullException(nameof(documentSession));
            _fixerClient = fixerClient
                ?? throw new ArgumentNullException(nameof(fixerClient));
            _configuration = configuration
                ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<(decimal, decimal)> Exchange(Currencies fromCurrency, Currencies toCurrency, decimal amount, decimal tax = 0)
        {
            if (fromCurrency == toCurrency)
                return (amount, 0);

            var (currencyRate, fromOpposite) = await GetExhangeRateByCurrency(fromCurrency, toCurrency);

            decimal exchangeResultNetto = 0;
            if (fromOpposite)
                exchangeResultNetto = Decimal.Divide(amount, currencyRate);
            if (!fromOpposite)
                exchangeResultNetto = Decimal.Multiply(amount, currencyRate);


            if (tax is not 0)
            {
                tax += Decimal.Multiply(exchangeResultNetto, tax);
                return (exchangeResultNetto + tax, tax);
            }

            return (decimal.Round(exchangeResultNetto, 2, MidpointRounding.AwayFromZero), decimal.Round(tax, 2, MidpointRounding.AwayFromZero));
        }

        public async Task UpsertCurrencyRate(CancellationToken cancellationToken = default)
        {
            // NOTE: this endpoint in fixer client works only with EUR because of subscription plan
            var fixerApiResult = await _fixerClient.GetLatestCurrencyRate(_configuration["FixerApiKey"], 1)
                        ?? throw new Exception();

            var lastCurrencyRateUpdate = await _documentSession
                    .Query<CurrencyRate>()
                    .OrderByDescending(x => x.FixerServerUpdate)
                    .FirstOrDefaultAsync(token: cancellationToken);

            Dictionary<Currencies, decimal> rates = new();
            foreach (KeyValuePair<string, decimal> rate in fixerApiResult.Rates)
            {
                if (Enum.TryParse<Currencies>(rate.Key, out Currencies currencyResult))
                    rates.Add(currencyResult, rate.Value);
            }
            if (lastCurrencyRateUpdate is null || fixerApiResult.BaseCurrency != lastCurrencyRateUpdate.Base.ToString())
            {
                await _documentSession.StoreAsync(new CurrencyRate
                {
                    CurrencyRateId = Guid.NewGuid(),
                    Base = Enum.Parse<Currencies>(fixerApiResult.BaseCurrency),
                    ServerUpdate = DateTime.UtcNow,
                    FixerServerUpdate = TimeStampConverter.UnixTimeStampToDateTime(fixerApiResult.Timestamp),
                    Rates = rates
                }, cancellationToken);
            }

            if (lastCurrencyRateUpdate is not null)
            {
                lastCurrencyRateUpdate.Update(rates, TimeStampConverter.UnixTimeStampToDateTime(fixerApiResult.Timestamp));
            }

            await _documentSession.SaveChangesAsync(cancellationToken);
        }

        private async Task<(decimal, bool)> GetExhangeRateByCurrency(Currencies formCurrency, Currencies toCurrency)
        {
            decimal currencyRate = 0;
            bool fromOpposite = false;

            currencyRate = await _documentSession.Query<CurrencyRate>()
                    .Where(x => x.Base == formCurrency)
                    .Select(x => x.Rates[toCurrency])
                    .FirstOrDefaultAsync();

            if (currencyRate == 0)
            {
                currencyRate = await _documentSession.Query<CurrencyRate>()
                    .Where(x => x.Base == toCurrency)
                    .Select(x => x.Rates[formCurrency])
                    .FirstOrDefaultAsync();

                if (currencyRate == 0)
                {
                    try
                    {
                        await UpsertCurrencyRate();
                        return await GetExhangeRateByCurrency(formCurrency, toCurrency);
                    }
                    catch
                    {
                        throw new CurrencyExchangeOperationException("The exchange operation cannot be performed");
                    }
                }
                fromOpposite = true;
                return (currencyRate, fromOpposite);
            }

            return (currencyRate, fromOpposite);
        }
    }
}
