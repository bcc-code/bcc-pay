using System;
using System.Collections.Generic;
using System.Linq;
using BccPay.Core.Domain;
using BccPay.Core.Domain.Entities;
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

            UpdatePaymentConditionConfigurations(_bccPaymentsConfiguration.Value.PaymentConfigurations);

            _documentSession.SaveChanges();
        }

        private void UpdatePaymentProviderDefinitions(List<BccPaymentConfiguration> paymentDefinitions)
        {
            var oldPaymentDefinitions = _documentSession.Query<PaymentProviderDefinition>().ToList();

            // add/update payment provider definition
            foreach (var definition in paymentDefinitions)
            {
                var oldDefinition = oldPaymentDefinitions.FirstOrDefault(x => x.PaymentDefinitionCode == definition.Id);
                if (oldDefinition != null)
                {
                    oldDefinition.Provider = definition.Provider;
                    oldDefinition.Settings = definition.Settings;
                }
                else
                {
                    _documentSession.Store(new PaymentProviderDefinition
                    {
                        PaymentDefinitionCode = definition.Id,
                        Provider = definition.Provider,
                        Settings = definition.Settings
                    });
                }
            }

            // remove old configurations
            oldPaymentDefinitions
                .Where(x => !paymentDefinitions.Any(c => c.Id == x.PaymentDefinitionCode))
                .ToList()
                .ForEach(_documentSession.Delete);
        }

        private void UpdatePaymentConditionConfigurations(List<PaymentConditionConfigurations> paymentConfigurations)
        {
            var existingConfigurations = _documentSession.Query<PaymentConditionConfigurations>().ToList();

            foreach (var paymentConfiguration in paymentConfigurations)
            {
                var existingPaymentConfiguration = existingConfigurations.FirstOrDefault(x => x.Conditions.CountryCode == paymentConfiguration.Conditions.CountryCode);
                if (existingPaymentConfiguration != null)
                {
                    existingPaymentConfiguration = paymentConfiguration;
                }
                else
                {
                    _documentSession.Store(new PaymentConfiguration
                    {
                        Conditions = new PaymentConditions
                        {
                            CountryCode = paymentConfiguration.Conditions.CountryCode,
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
