using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.Exceptions;
using BccPay.Core.Infrastructure.RefitClients;
using BccPay.Core.Shared.Converters;
using Microsoft.Extensions.Configuration;
using Raven.Client.Documents.Session;

namespace BccPay.Core.Infrastructure.Helpers.Implementation;

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

    public Task<CurrencyConversionRecord> Exchange(string fromCurrency, string toCurrency, decimal amount,
        decimal exchangeRateMarkup = 0)
    {
        if (Enum.TryParse(fromCurrency, out Currencies fromCurrencyResult) &&
            Enum.TryParse(toCurrency, out Currencies toCurrencyResult))
        {
            return Exchange(fromCurrencyResult, toCurrencyResult, amount, exchangeRateMarkup);
        }

        throw new CurrencyExchangeOperationException("Unable to convert values.");
    }

    public async Task<CurrencyConversionRecord> Exchange(Currencies fromCurrency, Currencies toCurrency,
        decimal amount, decimal exchangeMarkup = 0)
    {
        if (amount <= 0)
            throw new CurrencyExchangeOperationException("Unable to convert values.");

        if (fromCurrency == toCurrency)
            return new CurrencyConversionRecord(null,
                fromCurrency,
                toCurrency,
                0,
                amount,
                amount);

        (decimal currencyRate, DateTime? updateTime) =
            await GetExchangeRateByCurrency(fromCurrency, toCurrency);

        if (exchangeMarkup is not 0)
            currencyRate *= (1 + exchangeMarkup);

        decimal exchangeResult = Decimal.Multiply(amount, currencyRate);


        return new CurrencyConversionRecord(
            updateTime,
            fromCurrency,
            toCurrency,
            currencyRate,
            amount,
            exchangeResult);
    }

    public async Task UpsertByBaseCurrencyRate(Currencies currency = Currencies.NOK,
        CancellationToken cancellationToken = default)
    {
        var fixerApiResult =
            await _fixerClient.GetRatesByBaseCurrency(_configuration["FixerApiKey"], currency.ToString())
            ?? throw new Exception();

        var lastCurrencyRateUpdate = await _documentSession
            .LoadAsync<CurrencyRate>(CurrencyRate.GetCurrencyRateId(currency), cancellationToken);

        Dictionary<Currencies, decimal> rates = new();
        foreach ((string key, decimal value) in fixerApiResult.Rates)
        {
            if (Enum.TryParse(key, out Currencies currencyResult))
                rates.Add(currencyResult, value);
        }

        if (lastCurrencyRateUpdate is null ||
            fixerApiResult.BaseCurrency != lastCurrencyRateUpdate.BaseCurrency.ToString())
        {
            await _documentSession.StoreAsync(new CurrencyRate
            {
                BaseCurrency = Enum.Parse<Currencies>(fixerApiResult.BaseCurrency),
                ServerUpdate = DateTime.UtcNow,
                FixerServerUpdate = TimeStampConverter.UnixTimeStampToDateTime(fixerApiResult.Timestamp),
                Rates = rates
            }, cancellationToken);
        }

        lastCurrencyRateUpdate?.Update(rates,
            TimeStampConverter.UnixTimeStampToDateTime(fixerApiResult.Timestamp));

        await _documentSession.SaveChangesAsync(cancellationToken);
    }

    public async Task<(decimal, DateTime?)> GetExchangeRateByCurrency(Currencies fromCurrency,
        Currencies toCurrency)
    {
        var result = await _documentSession
            .LoadAsync<CurrencyRate>(CurrencyRate.GetCurrencyRateId(fromCurrency));

        if (IsCurrencyRateValid(result, 2))
        {
            return (result.Rates[toCurrency], result.FixerServerUpdate);
        }

        try
        {
            await UpsertByBaseCurrencyRate(fromCurrency);
            return await GetExchangeRateByCurrency(fromCurrency, toCurrency);
        }
        catch
        {
            throw new CurrencyExchangeOperationException("The exchange operation cannot be performed");
        }
    }

    private static bool IsCurrencyRateValid(CurrencyRate currencyRate, int expirationTimeInHours)
        => currencyRate is not null &&
           DateTime.UtcNow.Subtract(currencyRate.ServerUpdate).TotalHours < expirationTimeInHours;
}
