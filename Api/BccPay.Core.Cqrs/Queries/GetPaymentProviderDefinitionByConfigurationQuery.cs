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
using Raven.Client.Documents.Session;

namespace BccPay.Core.Cqrs.Queries
{
    public record GetPaymentProviderDefinitionByConfigurationQuery(
        string CountryCode,
        string[] PaymentTypes = null) : IRequest<AvailableConfigurationResult>;

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
            var query = _documentSession.Query<PaymentConfiguration>();

            if (request.PaymentTypes is not null)
                query.Where(paymentConfiguration => paymentConfiguration.Conditions.PaymentTypes.ContainsAny(request.PaymentTypes));

            var paymentConfigurations = await query.Where(paymentConfiguration
                                                => request.CountryCode == paymentConfiguration.Conditions.CountryCode
                                                || paymentConfiguration.Conditions.CountryCode == Country.DefaultCountryCode)
                                            .ToListAsync(cancellationToken);

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

            // NOTE: not relevant, always takes default values as result
            // if (paymentProviderDefinition is null)
            //    throw new InvalidConfigurationException($"Unable to find configuration for {request.CountryCode} with type {string.Join(", ", request.PaymentTypes)}");


            List<string> paymentTypes = new();

            if (request.PaymentTypes is not null && paymentConfigurations.Count > 0)
            {
                paymentTypes = paymentConfigurations.SelectMany(configuration
                            => configuration.Conditions.PaymentTypes.Intersect(request.PaymentTypes))
                        .Distinct()
                        .ToList();
            }

            if (request.PaymentTypes is null)
            {
                foreach (var paymentConfiguration in paymentConfigurations)
                {
                    foreach (var paymentTypesArray in paymentConfiguration.Conditions.PaymentTypes)
                    {
                        paymentTypes.Add(paymentTypesArray);
                    }
                }
            }

            return new AvailableConfigurationResult(paymentTypes.Distinct().ToList(), paymentProviderDefinition);
        }
    }
}
