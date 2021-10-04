﻿using System;
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
        Currencies? CurrencyCode = null) : IRequest<AvailableConfigurationsResult>;

    public record AvailableConfigurationsResult(List<PaymentConfigurationResult> PaymentConfigurations);

    public class PaymentConfigurationResult
    {
        public string CountryCode { get; set; }
        public List<PaymentProviderDefinitionResult> ProviderDefinitionDetails { get; set; }
        public string[] PaymentTypes { get; set; }
        public string[] CurrencyCodes { get; set; }

        internal string[] PaymentProviderDefinitionIds { get; set; }
    }

    public class PaymentProviderDefinitionResult
    {
        public string Id { get; set; }
        public PaymentProvider PaymentProvider { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public Currencies Currency { get; set; }

        internal decimal Markup { get; set; }
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
            var query = _documentSession.Query<PaymentConfiguration>();

            if (!string.IsNullOrWhiteSpace(request.CountryCode))
            {
                string countryCodeIso3 = AddressConverter.ConvertCountry(request.CountryCode, CountryCodeFormat.Alpha3);

                query = query.Where(paymentConfiguration
                             => paymentConfiguration.CountryCode.In(new string[]
                             {
                                 countryCodeIso3, Country.DefaultCountryCode
                             }));
            }

            if (!string.IsNullOrWhiteSpace(request.PaymentType))
                query = query.Where(paymentConfiguration => paymentConfiguration.Conditions.PaymentTypes.Contains(request.PaymentType));
            if (request.CurrencyCode is not null)
                query = query.Where(paymentConfiguration => paymentConfiguration.Conditions.CurrencyCodes.Contains(request.CurrencyCode.ToString()));

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
                .SelectMany(paymentConfiguration => paymentConfiguration.PaymentProviderDefinitionIds)
                .ToArray();

            var paymentProviderDefinition = await _documentSession.Query<PaymentProviderDefinition>()
                     .Where(providerDefinition => providerDefinition.PaymentDefinitionCode.In(idsToCompare))
                     .Select(providerDefinition
                     => new PaymentProviderDefinitionResult
                     {
                         Id = providerDefinition.PaymentDefinitionCode,
                         PaymentProvider = providerDefinition.Provider,
                         PaymentMethod = providerDefinition.Settings.PaymentMethod,
                         Markup = providerDefinition.Settings.Markup,
                         Currency = providerDefinition.Settings.Currency
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
