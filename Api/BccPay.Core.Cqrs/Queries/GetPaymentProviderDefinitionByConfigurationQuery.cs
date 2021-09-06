using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BccPay.Core.DataAccess.Indexes;
using BccPay.Core.Domain;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;

namespace BccPay.Core.Cqrs.Queries
{
    public record GetPaymentProviderDefinitionByConfigurationQuery(
        string CountryCode,
        string PaymentType = null,
        string CurrencyCode = null) : IRequest<AvailableConfigurationResult>;

    public record AvailableConfigurationResult(List<string> SupportedTypes, List<PaymentConfigurationResult> PaymentConfigurations);

    public class PaymentConfigurationResult
    {
        public string PaymentConfigurationId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentProvider Provider { get; set; }
        public Currencies Currency { get; set; }
    }

    public class GetCountryPaymentConfigurationsQueryHandler : IRequestHandler<GetPaymentProviderDefinitionByConfigurationQuery, AvailableConfigurationResult>
    {
        private readonly IAsyncDocumentSession _documentSession;

        public GetCountryPaymentConfigurationsQueryHandler(IAsyncDocumentSession documentSession)
        {
            _documentSession = documentSession
                ?? throw new ArgumentNullException(nameof(documentSession));
        }

        public async Task<AvailableConfigurationResult> Handle(GetPaymentProviderDefinitionByConfigurationQuery request, CancellationToken cancellationToken)
        {
            var query = _documentSession.Query<PaymentConfigurationIndex.Result, PaymentConfigurationIndex>()
                        .Where(paymentConfiguration => paymentConfiguration.CountryCode == request.CountryCode);

            if (!string.IsNullOrWhiteSpace(request.CurrencyCode))
                query = query.Search(paymentConfiguration => paymentConfiguration.SearchContent, request.CurrencyCode, options: SearchOptions.And);
            if (!string.IsNullOrWhiteSpace(request.PaymentType))
                query = query.Search(paymentConfiguration => paymentConfiguration.SearchContent, request.PaymentType, options: SearchOptions.And);

            List<PaymentConfigurationIndex.Result> paymentConfigurations = new();

            paymentConfigurations = await query.ToListAsync(cancellationToken);

            if (paymentConfigurations.Count == 0)
            {
                paymentConfigurations = await _documentSession
                    .Query<PaymentConfigurationIndex.Result, PaymentConfigurationIndex>()
                    .Where(paymentConfiguration => paymentConfiguration.CountryCode == Country.DefaultCountryCode)
                    .ToListAsync(cancellationToken);
            }

            var idsToCompare = paymentConfigurations.SelectMany(x => x.PaymentProviderDefinitionIds).ToArray();

            var paymentProviderDefinition = await _documentSession.Query<PaymentProviderDefinition>()
                     .Where(x => x.PaymentDefinitionCode.In(idsToCompare))
                     .Select(definition => new PaymentConfigurationResult
                     {
                         PaymentConfigurationId = definition.PaymentDefinitionCode,
                         Provider = definition.Provider,
                         Currency = definition.Settings.Currency,
                         PaymentMethod = definition.Settings.PaymentMethod
                     })
                     .ToListAsync(cancellationToken);

            List<string> supportedPaymentTypes = new();

            if (request.PaymentType is not null && paymentConfigurations.Count > 0)
            {
                supportedPaymentTypes = paymentConfigurations.Where(configuration
                            => request.PaymentType.In(configuration.Conditions.PaymentTypes))
                        .SelectMany(configuration => configuration.Conditions.PaymentTypes)
                        .Distinct()
                        .ToList();
            }

            if (supportedPaymentTypes.Count == 0)
            {
                foreach (var paymentConfiguration in paymentConfigurations)
                {
                    foreach (var paymentTypesArray in paymentConfiguration.Conditions.PaymentTypes)
                    {
                        supportedPaymentTypes.Add(paymentTypesArray);
                    }
                }
            }

            return new AvailableConfigurationResult(supportedPaymentTypes.Distinct().ToList(), paymentProviderDefinition);
        }
    }
}
