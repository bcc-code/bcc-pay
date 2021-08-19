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
            UpdatePaymentConfigurations(_bccPaymentsConfiguration.Value.PaymentConfigurations);

            UpdateCountries(_bccPaymentsConfiguration.Value.CountryPaymentConfigurations);

            _documentSession.SaveChanges();
        }

        private void UpdatePaymentConfigurations(List<BccPaymentConfiguration> paymentConfigs)
        {
            var oldPaymentConfigs = _documentSession.Query<PaymentConfiguration>().ToList();

            // add/update payment configurations
            foreach (var config in paymentConfigs)
            {
                var oldConfig = oldPaymentConfigs.FirstOrDefault(x => x.PaymentConfigurationCode == config.Id);
                if (oldConfig != null)
                {
                    oldConfig.Provider = config.Provider;
                    oldConfig.Settings = config.Settings;
                }
                else
                {
                    _documentSession.Store(new PaymentConfiguration
                    {
                        PaymentConfigurationCode = config.Id,
                        Provider = config.Provider,
                        Settings = config.Settings
                    });
                }
            }

            // remove old configurations
            oldPaymentConfigs
                .Where(x => !paymentConfigs.Any(c => c.Id == x.PaymentConfigurationCode))
                .ToList()
                .ForEach(_documentSession.Delete);
        }

        private void UpdateCountries(List<CountryBccPaymentConfigurations> countryPaymentConfigurations)
        {
            var existingCountries = _documentSession.Query<Country>().ToList();

            // do not remove country if it isn't specified, just remove payment configurations
            existingCountries.ForEach(c => c.PaymentConfigurations = Array.Empty<string>());

            foreach (var config in countryPaymentConfigurations)
            {
                var existingConfig = existingCountries.FirstOrDefault(x => x.CountryCode == config.CountryCode);
                if (existingConfig != null)
                {
                    existingConfig.PaymentConfigurations = config.PaymentConfigurationIds;
                }
                else
                {
                    _documentSession.Store(new Country
                    {
                        CountryCode = config.CountryCode,
                        PaymentConfigurations = config.PaymentConfigurationIds
                    });
                }
            }
        }
    }
}
