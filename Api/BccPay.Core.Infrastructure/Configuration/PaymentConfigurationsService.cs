using System;
using System.Collections.Generic;
using System.Linq;
using BccPay.Core.Domain;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Infrastructure.BccPaymentSettings;
using BccPay.Core.Shared.Converters;
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
        private readonly InternalSettings _internalSettings;

        public PaymentConfigurationsService(IDocumentSession documentSession,
            IOptions<BccPaymentsConfiguration> bccPaymentsConfiguration, InternalSettings internalSettings)
        {
            _documentSession = documentSession
                               ?? throw new ArgumentNullException(nameof(documentSession));

            _bccPaymentsConfiguration = bccPaymentsConfiguration;

            _internalSettings = internalSettings;
        }

        public void InitPaymentsConfiguration()
        {
            UpdatePaymentProviderDefinitions(_bccPaymentsConfiguration.Value.PaymentProviderDefinitions);

            UpdatePaymentConfigurations(_bccPaymentsConfiguration.Value.PaymentConfigurations);

            _documentSession.SaveChanges();
        }

        private void UpdatePaymentProviderDefinitions(List<PaymentProviderDefinitionModel> paymentDefinitions)
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
                    oldDefinition.Settings.Markup = definition.Settings.Markup;
                }
                else
                {
                    _documentSession.Store(new PaymentProviderDefinition
                    {
                        PaymentDefinitionCode = definition.Id,
                        Provider = definition.Provider,
                        Settings = new PaymentSetting
                        {
                            Currency = definition.Settings.Currency,
                            PaymentMethod = definition.Settings.PaymentMethod,
                            Markup = definition.Settings.Markup
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

        private void UpdatePaymentConfigurations(List<PaymentConfigurationModel> paymentConfigurations)
        {
            var existingConfigurations = _documentSession.Query<PaymentConfiguration>().ToList();

            foreach (var paymentConfiguration in paymentConfigurations)
            {
                paymentConfiguration.CountryCode = AddressConverter.ConvertCountry(paymentConfiguration.CountryCode,
                    _internalSettings.StoreCountryCodeFormat);
                
                var existingPaymentConfiguration =
                    existingConfigurations.FirstOrDefault(x => x.CountryCode == paymentConfiguration.CountryCode);

                if (existingPaymentConfiguration != null &&
                    !existingPaymentConfiguration.EqualsInJson(paymentConfiguration))
                {
                    existingPaymentConfiguration.Conditions.CurrencyCodes =
                        paymentConfiguration.Conditions.CurrencyCodes;
                    existingPaymentConfiguration.Conditions.PaymentTypes = paymentConfiguration.Conditions.PaymentTypes;
                    existingPaymentConfiguration.PaymentProviderDefinitionIds =
                        paymentConfiguration.PaymentProviderDefinitionIds;
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