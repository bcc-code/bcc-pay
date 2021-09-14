using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BccPay.Core.Domain;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using BccPay.Core.Shared.Converters;
using BccPay.Core.Shared.Helpers;
using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;

namespace BccPay.Core.Cqrs.Queries
{
    public record GetPaymentConfigurationsByQuery(
        string CountryCode,
        string PaymentType = null,
        string CurrencyCode = null) : IRequest<AvailableConfigurationsResult>;

    public record AvailableConfigurationsResult(List<PaymentConfigurationResult> PaymentConfigurations);

    public class PaymentConfigurationResult
    {
        public string CountryCode { get; set; }
        internal string[] PaymentProviderDefinitionIds { get; set; }
        public List<PaymentProviderDefinitionResult> ProviderDefinitionDetails { get; set; }
        public string[] PaymentTypes { get; set; }
        public string[] CurrencyCodes { get; set; }
    }

    public class PaymentProviderDefinitionResult
    {
        public string Id { get; set; }
        public PaymentProvider PaymentProvider { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }

    public class GetPaymentConfigurationsByQueryHandler : IRequestHandler<GetPaymentConfigurationsByQuery, AvailableConfigurationsResult>
    {
        private readonly IAsyncDocumentSession _documentSession;

        public GetPaymentConfigurationsByQueryHandler(IAsyncDocumentSession documentSession)
        {
            _documentSession = documentSession
                ?? throw new ArgumentNullException(nameof(documentSession));
        }

        public async Task<AvailableConfigurationsResult> Handle(GetPaymentConfigurationsByQuery request, CancellationToken cancellationToken)
        {
            var countryCodeAlpha3 = AddressConverter.ConvertCountry(request.CountryCode, CountryCodeFormat.Alpha3);

            var query = _documentSession.Query<PaymentConfiguration>()
                        .Where(paymentConfiguration
                            => paymentConfiguration.CountryCode.In(new string[] { countryCodeAlpha3, Domain.Entities.Country.DefaultCountryCode }));

            if (!string.IsNullOrWhiteSpace(request.PaymentType))
                query = query.Where(paymentConfiguration => paymentConfiguration.Conditions.PaymentTypes.Contains(request.PaymentType));
            if (!string.IsNullOrWhiteSpace(request.CurrencyCode))
                query = query.Where(paymentConfiguration => paymentConfiguration.Conditions.CurrencyCodes.Contains(request.CurrencyCode));

            List<PaymentConfigurationResult> paymentConfigurations = new();

            paymentConfigurations = await query.Select(paymentConfiguration
                    => new PaymentConfigurationResult
                    {
                        CountryCode = paymentConfiguration.CountryCode,
                        CurrencyCodes = paymentConfiguration.Conditions.CurrencyCodes,
                        PaymentTypes = paymentConfiguration.Conditions.PaymentTypes,
                        PaymentProviderDefinitionIds = paymentConfiguration.PaymentProviderDefinitionIds
                    }).ToListAsync(cancellationToken);

            var idsToCompare = paymentConfigurations
                .SelectMany(x => x.PaymentProviderDefinitionIds)
                .ToArray();

            var paymentProviderDefinition = await _documentSession.Query<PaymentProviderDefinition>()
                     .Where(x => x.PaymentDefinitionCode.In(idsToCompare))
                     .Select(x
                     => new PaymentProviderDefinitionResult
                     {
                         Id = x.PaymentDefinitionCode,
                         PaymentProvider = x.Provider,
                         PaymentMethod = x.Settings.PaymentMethod
                     })
                     .ToListAsync(cancellationToken);

            foreach (var paymentConfiguration in paymentConfigurations)
            {
                paymentConfiguration.ProviderDefinitionDetails = paymentProviderDefinition.Where(x => paymentConfiguration.PaymentProviderDefinitionIds.Contains(x.Id)).ToList();
            }

            return new AvailableConfigurationsResult(paymentConfigurations);
        }
    }
}
