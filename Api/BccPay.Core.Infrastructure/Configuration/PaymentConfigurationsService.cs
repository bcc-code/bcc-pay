using System;
using System.Collections.Generic;
using System.Linq;
using BccPay.Core.Domain;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Shared.Helpers;
using Microsoft.Extensions.Options;
using Raven.Client.Documents.Session;

namespace BccPay.Core.Infrastructure.Configuration
{
    public interface IPaymentConfigurationsService
    {
        void InitPaymentsConfiguration();
    }

    internal class PaymentConfigurationsService : IPaymentConfigurationsService
    {
        private readonly IDocumentSession _documentSession;
        private readonly IOptions<BccPaymentsConfiguration> _bccPaymentsConfiguration;

        public PaymentConfigurationsService(IDocumentSession documentSession, IOptions<BccPaymentsConfiguration> bccPaymentsConfiguration)
        {
            _documentSession = documentSession
                ?? throw new ArgumentNullException(nameof(documentSession));

            _bccPaymentsConfiguration = bccPaymentsConfiguration;
        }

        public void InitPaymentsConfiguration()
        {
            UpdatePaymentProviderDefinitions(_bccPaymentsConfiguration.Value.PaymentProviderDefinitions);

            UpdatePaymentConfigurations(_bccPaymentsConfiguration.Value.PaymentConfigurations);

            _documentSession.SaveChanges();
        }

        private void UpdatePaymentProviderDefinitions(List<PaymentProviderDefinitions> paymentDefinitions)
        {
            var oldPaymentDefinitions = _documentSession.Query<PaymentProviderDefinition>().ToList();

            // add/update payment provider definition
            foreach (var definition in paymentDefinitions)
            {
                var oldDefinition = oldPaymentDefinitions.FirstOrDefault(x => x.PaymentDefinitionCode == definition.Id);
                if (oldDefinition != null)
                {
                    oldDefinition.Provider = definition.Provider;
                    oldDefinition.Settings.Currency = definition.Settings.Currency;
                    oldDefinition.Settings.PaymentMethod = definition.Settings.PaymentMethod;
                }
                else
                {
                    _documentSession.Store(new PaymentProviderDefinition
                    {
                        PaymentDefinitionCode = definition.Id,
                        Provider = definition.Provider,
                        Settings = new Domain.PaymentSettings
                        {
                            Currency = definition.Settings.Currency,
                            PaymentMethod = definition.Settings.PaymentMethod
                        }
                    });
                }
            }

            // remove old configurations
            oldPaymentDefinitions
                .Where(x => !paymentDefinitions.Any(c => c.Id == x.PaymentDefinitionCode))
                .ToList()
                .ForEach(_documentSession.Delete);
        }

        private void UpdatePaymentConfigurations(List<PaymentConfigurations> paymentConfigurations)
        {
            var existingConfigurations = _documentSession.Query<PaymentConfiguration>().ToList();

            foreach (var paymentConfiguration in paymentConfigurations)
            {
                var existingPaymentConfiguration = existingConfigurations.FirstOrDefault(x => x.CountryCode == paymentConfiguration.CountryCode);

                if (existingPaymentConfiguration != null && !existingPaymentConfiguration.EqualsInJson(paymentConfiguration))
                {
                    existingPaymentConfiguration.Conditions.CurrencyCodes = paymentConfiguration.Conditions.CurrencyCodes;
                    existingPaymentConfiguration.Conditions.PaymentTypes = paymentConfiguration.Conditions.PaymentTypes;
                    existingPaymentConfiguration.PaymentProviderDefinitionIds = paymentConfiguration.PaymentProviderDefinitionIds;
                }

                if (existingPaymentConfiguration is null)
                {
                    _documentSession.Store(new PaymentConfiguration
                    {
                        CountryCode = paymentConfiguration.CountryCode,
                        Conditions = new PaymentConditions
                        {
                            CurrencyCodes = paymentConfiguration.Conditions.CurrencyCodes,
                            PaymentTypes = paymentConfiguration.Conditions.PaymentTypes
                        },
                        PaymentProviderDefinitionIds = paymentConfiguration.PaymentProviderDefinitionIds
                    });
                }
            }
        }
    }
}
