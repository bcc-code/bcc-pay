using System;
using System.Threading;
using System.Threading.Tasks;
using BccPay.Core.Domain;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.Exceptions;
using BccPay.Core.Infrastructure.Helpers;
using BccPay.Core.Infrastructure.RefitClients;
using BccPay.Core.Sample.Contracts.Responses;
using MediatR;
using Raven.Client.Documents.Session;

namespace BccPay.Core.Cqrs.Queries
{
    public record GetCurrencyExchangeByDefinitionIdQuery
        (string DefinitionId, Currencies? FromCurrency, Currencies? ToCurrency) : IRequest<
            GetCurrencyExchangeByDefinitionResponse>;

    public class GetCurrencyExchangeByDefinitionIdQueryHandler : IRequestHandler<GetCurrencyExchangeByDefinitionIdQuery,
        GetCurrencyExchangeByDefinitionResponse>
    {
        private readonly IAsyncDocumentSession _documentSession;
        private readonly ICurrencyService _currencyService;
        private readonly IFixerClient _fixerClient;

        public GetCurrencyExchangeByDefinitionIdQueryHandler(IAsyncDocumentSession documentSession,
            ICurrencyService currencyService,
            IFixerClient fixerClient)
        {
            _documentSession = documentSession
                               ?? throw new ArgumentNullException(nameof(documentSession));
            _currencyService = currencyService
                               ?? throw new ArgumentNullException(nameof(currencyService));
            _fixerClient = fixerClient
                           ?? throw new ArgumentNullException(nameof(fixerClient));
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
            Currencies toCurrency = request.ToCurrency ?? providerDefinition.Settings.Currency;
            decimal rate = 0;

            (decimal exchangeRate, bool fromOpposite, DateTime? _) =
                await _currencyService.GetExchangeRateByCurrency(fromCurrency, toCurrency);

            rate = fromOpposite ? exchangeRate * 100 : exchangeRate;
            rate += providerDefinition.Settings.Markup;

            return new GetCurrencyExchangeByDefinitionResponse(fromCurrency, toCurrency, rate);
        }
    }
}