using System;
using System.Threading;
using System.Threading.Tasks;
using BccPay.Core.Domain;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.Exceptions;
using BccPay.Core.Infrastructure.Helpers;
using BccPay.Core.Sample.Contracts.Responses;
using MediatR;
using Raven.Client.Documents.Session;

namespace BccPay.Core.Cqrs.Queries
{
    public record GetCurrencyExchangeByDefinitionIdQuery
        (string DefinitionId, Currencies? FromCurrency) : IRequest<
            GetCurrencyExchangeByDefinitionResponse>;

    public class GetCurrencyExchangeByDefinitionIdQueryHandler : IRequestHandler<GetCurrencyExchangeByDefinitionIdQuery,
        GetCurrencyExchangeByDefinitionResponse>
    {
        private readonly IAsyncDocumentSession _documentSession;
        private readonly ICurrencyService _currencyService;

        public GetCurrencyExchangeByDefinitionIdQueryHandler(IAsyncDocumentSession documentSession,
            ICurrencyService currencyService)
        {
            _documentSession = documentSession
                               ?? throw new ArgumentNullException(nameof(documentSession));
            _currencyService = currencyService
                               ?? throw new ArgumentNullException(nameof(currencyService));
        }

        public async Task<GetCurrencyExchangeByDefinitionResponse> Handle(
            GetCurrencyExchangeByDefinitionIdQuery request,
            CancellationToken cancellationToken)
        {
            var providerDefinition = await _documentSession.LoadAsync<PaymentProviderDefinition>(
                                         PaymentProviderDefinition.GetDocumentId(request.DefinitionId),
                                         cancellationToken)
                                     ?? throw new NotFoundException("No definition found");

            Currencies fromCurrency = request.FromCurrency ?? Currencies.NOK;
            Currencies toCurrency = providerDefinition.Settings.Currency;

            (decimal exchangeRate, bool _, DateTime? _) =
                await _currencyService.GetExchangeRateByCurrency(fromCurrency, toCurrency);

            exchangeRate *= (1 + providerDefinition.Settings.Markup);

            return new GetCurrencyExchangeByDefinitionResponse(fromCurrency, toCurrency, exchangeRate);
        }
    }
}