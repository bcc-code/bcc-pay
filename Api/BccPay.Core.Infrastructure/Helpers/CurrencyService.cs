using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.Dtos;
using BccPay.Core.Infrastructure.Exceptions;
using BccPay.Core.Infrastructure.RefitClients;
using BccPay.Core.Shared.Converters;
using Microsoft.Extensions.Configuration;
using Raven.Client.Documents.Session;

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

        public async Task<CurrencyExchangeResult> Exchange(Currencies fromCurrency, Currencies toCurrency, decimal amount, decimal exchangeRateMarkup = 0)
        {
            if (fromCurrency == toCurrency || amount <= 0)
                throw new CurrencyExchangeOperationException("Unable to convert values.");

            var (currencyRate, fromOpposite) = await GetExhangeRateByCurrency(fromCurrency, toCurrency);

            decimal exchangeResultNetto = 0;
            if (fromOpposite)
                exchangeResultNetto = Decimal.Divide(amount, currencyRate);
            if (!fromOpposite)
                exchangeResultNetto = Decimal.Multiply(amount, currencyRate);


            if (exchangeRateMarkup is not 0)
            {
                exchangeRateMarkup = Decimal.Multiply(exchangeResultNetto, exchangeRateMarkup);
                decimal exchangeResultGross = exchangeResultNetto + exchangeRateMarkup;

                return new CurrencyExchangeResult(
                fromCurrency,
                toCurrency,
                amount,
                exchangeResultGross.TwoDigitsAfterPoint(),
                exchangeResultNetto.TwoDigitsAfterPoint(),
                exchangeRateMarkup.TwoDigitsAfterPoint());
            }

            return new CurrencyExchangeResult(
                fromCurrency,
                toCurrency,
                amount,
                exchangeResultNetto.TwoDigitsAfterPoint(),
                exchangeResultNetto.TwoDigitsAfterPoint(),
                exchangeRateMarkup.TwoDigitsAfterPoint());
        }

        public async Task UpsertByBaseCurrencyRate(Currencies currency = Currencies.NOK, CancellationToken cancellationToken = default)
        {
            var fixerApiResult = await _fixerClient.GetRatesByBaseCurrency(_configuration["FixerApiKey"], currency.ToString())
                        ?? throw new Exception();

            var lastCurrencyRateUpdate = await _documentSession
                    .LoadAsync<CurrencyRate>(CurrencyRate.GetCurrencyRateId(currency), cancellationToken);

            Dictionary<Currencies, decimal> rates = new();
            foreach (KeyValuePair<string, decimal> rate in fixerApiResult.Rates)
            {
                if (Enum.TryParse<Currencies>(rate.Key, out Currencies currencyResult))
                    rates.Add(currencyResult, rate.Value);
            }

            if (lastCurrencyRateUpdate is null || fixerApiResult.BaseCurrency != lastCurrencyRateUpdate.BaseCurrency.ToString())
            {
                await _documentSession.StoreAsync(new CurrencyRate
                {
                    BaseCurrency = Enum.Parse<Currencies>(fixerApiResult.BaseCurrency),
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

        private async Task<(decimal, bool)> GetExhangeRateByCurrency(Currencies fromCurrency, Currencies toCurrency)
        {
            var result = await _documentSession
                    .LoadAsync<CurrencyRate>(CurrencyRate.GetCurrencyRateId(fromCurrency));

            if (!IsCurrencyRateValid(result, 2))
            {
                var oppositeResult = await _documentSession
                    .LoadAsync<CurrencyRate>(CurrencyRate.GetCurrencyRateId(toCurrency));

                if (!IsCurrencyRateValid(oppositeResult, 2))
                {
                    try
                    {
                        await UpsertByBaseCurrencyRate(fromCurrency);
                        return await GetExhangeRateByCurrency(fromCurrency, toCurrency);
                    }
                    catch
                    {
                        throw new CurrencyExchangeOperationException("The exchange operation cannot be performed");
                    }
                }

                return (oppositeResult.Rates[fromCurrency], true);
            }

            return (result.Rates[toCurrency], false);
        }

        private bool IsCurrencyRateValid(CurrencyRate currencyRate, int expirationTimeInHours)
        {
            if (currencyRate is null)
                return false;

            return DateTime.UtcNow.Subtract(currencyRate.ServerUpdate).TotalHours < expirationTimeInHours;
        }
    }
}
