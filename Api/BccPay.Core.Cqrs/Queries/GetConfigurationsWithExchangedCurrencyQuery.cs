using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.Helpers;
using BccPay.Core.Shared.Converters;
using MediatR;

namespace BccPay.Core.Cqrs.Queries;

public record GetConfigurationsWithExchangedCurrencyQuery(decimal Amount,
    Currencies? FromCurrency,
    Currencies? ToCurrency,
    string CountryCode,
    string PaymentType) : IRequest<GetConfigurationsWithExchangedCurrencyQueryResult>;

public record GetConfigurationsWithExchangedCurrencyQueryResult(HashSet<ProviderDefinitionExchangeDefinition> ProviderDefinitionExchangeDefinition);

public record ProviderDefinitionExchangeDefinition(string DefinitionId,
    PaymentProvider PaymentProvider,
    PaymentMethod PaymentMethod,
    Currencies? FromCurrency,
    Currencies? ToCurrency,
    decimal FromAmount,
    decimal ToAmount);

public class GetConfigurationsWithExchangedCurrencyQueryResultHandler : IRequestHandler<GetConfigurationsWithExchangedCurrencyQuery, GetConfigurationsWithExchangedCurrencyQueryResult>
{
    private readonly ICurrencyService _currencyService;
    private readonly IMediator _mediator;

    public GetConfigurationsWithExchangedCurrencyQueryResultHandler(ICurrencyService currencyService,
        IMediator mediator)
    {
        _currencyService = currencyService;
        _mediator = mediator;
    }

    public async Task<GetConfigurationsWithExchangedCurrencyQueryResult> Handle(GetConfigurationsWithExchangedCurrencyQuery request, CancellationToken cancellationToken)
    {
        var configurationQuery = new GetPaymentConfigurationsByQuery(request.CountryCode, request.PaymentType, request.ToCurrency);
        var configurationResult = await _mediator.Send(configurationQuery, cancellationToken);

        var combinedResult = new HashSet<ProviderDefinitionExchangeDefinition>();

        foreach (var configurations in configurationResult.PaymentConfigurations)
        {
            foreach (var definitionId in configurations.PaymentProviderDefinitionIds)
            {
                var details = configurations.ProviderDefinitionDetails.First(x => x.Id == definitionId);

                CurrencyConversionRecord currencyConversionResult = null;

                var fromCurrencyValue = request.FromCurrency ?? default;
                if (request.Amount > 0)
                    currencyConversionResult = await _currencyService.Exchange(fromCurrencyValue, details.Currency, request.Amount, details.Markup);

                combinedResult.Add(new ProviderDefinitionExchangeDefinition(definitionId,
                    details.PaymentProvider,
                    details.PaymentMethod,
                    fromCurrencyValue,
                    details.Currency,
                    request.Amount.ToAmountOfDigitsAfterPoint(),
                    currencyConversionResult?.ToAmount.ToAmountOfDigitsAfterPoint() ?? request.Amount.ToAmountOfDigitsAfterPoint()));
            }
        }

        return new GetConfigurationsWithExchangedCurrencyQueryResult(combinedResult);
    }
}
