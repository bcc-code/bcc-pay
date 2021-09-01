using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BccPay.Core.Domain;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Queries;
using Raven.Client.Documents.Session;

namespace BccPay.Core.Cqrs.Queries
{
    public record GetPaymentProviderDefinitionByConfigurationQuery(
        string CountryCode,
        string[] PaymentType = null) : IRequest<AvailableConfigurationResult>;

    public record AvailableConfigurationResult(string[] Types, List<PaymentConfigurationResult> PaymentConfigurations);

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
            var configurations = await _documentSession.Query<PaymentConfiguration>()
                                    .Where(paymentConfiguration
                                        => request.CountryCode == paymentConfiguration.Conditions.CountryCode
                                        && paymentConfiguration.Conditions.PaymentTypes.ContainsAny(request.PaymentType)
                                        || paymentConfiguration.Conditions.CountryCode == Country.DefaultCountryCode)
                                    .ToListAsync(cancellationToken);

            // if result have 0 configurations => return default
            // or 
            // if (configurations.Count == 0)
            //     throw new InvalidConfigurationException($"Unable to find payment implementation for {request.CountryCode} with type {string.Join(", ", request.PaymentType)}");

            var paymentProviderDefinition = await _documentSession.Query<PaymentProviderDefinition>()
                     .Search(definition => definition.PaymentDefinitionCode, configurations.SelectMany(configuration => configuration.PaymentProviderDefinitionIds), @operator: SearchOperator.And)
                     .Select(definition => new PaymentConfigurationResult
                     {
                         PaymentConfigurationId = definition.PaymentDefinitionCode,
                         Provider = definition.Provider,
                         Currency = definition.Settings.Currency,
                         PaymentMethod = definition.Settings.PaymentMethod
                     })
                     .ToListAsync(cancellationToken);

            var availableTypes = configurations.SelectMany(configuration => configuration.Conditions.PaymentTypes.Intersect(request.PaymentType));

            return new AvailableConfigurationResult(availableTypes.ToArray(), paymentProviderDefinition);
        }
    }
}
