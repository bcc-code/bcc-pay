﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BccPay.Core.Domain;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.Exceptions;
using BccPay.Core.Infrastructure.Helpers;
using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace BccPay.Core.Cqrs.Queries
{
    public class GetExchangedCurrencyQuery : IRequest<ExchangeResult>
    {
        public GetExchangedCurrencyQuery(
            decimal amount,
            Currencies fromCurrency,
            Currencies toCurrency,
            PaymentMethod paymentMethod)
        {
            Amount = amount;
            FromCurrency = fromCurrency;
            PaymentMethod = paymentMethod;
            ToCurrency = toCurrency;
        }

        public PaymentMethod PaymentMethod { get; set; }
        public decimal Amount { get; set; }
        public Currencies FromCurrency { get; set; }
        public Currencies ToCurrency { get; set; }
    }


    public record ExchangeResult(Currencies FromCurrency, Currencies ToCurrency, decimal FromAmount, decimal ToAmount);

    public class GetExchangedCurrencyQueryHandler : IRequestHandler<GetExchangedCurrencyQuery, ExchangeResult>
    {
        private readonly IAsyncDocumentSession _documentSession;
        private readonly ICurrencyService _currencyService;

        public GetExchangedCurrencyQueryHandler(IAsyncDocumentSession documentSession, ICurrencyService currencyService)
        {
            _documentSession = documentSession;
            _currencyService = currencyService;
        }

        public async Task<ExchangeResult> Handle(GetExchangedCurrencyQuery request, CancellationToken cancellationToken)
        {
            var providerDefinition = await _documentSession.Query<PaymentProviderDefinition>()
                .Where(x
                    => x.Settings.PaymentMethod == request.PaymentMethod
                    && x.Settings.Currency == request.ToCurrency)
                .FirstOrDefaultAsync(token: cancellationToken)
                ?? throw new InvalidConfigurationException($"Unsupported currency or method");           

            if (providerDefinition.Settings.Currency == request.FromCurrency)
                return new ExchangeResult(request.FromCurrency, providerDefinition.Settings.Currency, request.Amount, request.Amount);

            var result = await _currencyService.Exchange(request.FromCurrency, providerDefinition.Settings.Currency, request.Amount, providerDefinition.Settings.Markup);

            return new ExchangeResult(result.FromCurrency, result.ToCurrency, request.Amount, result.ToAmount);
        }
    }
}