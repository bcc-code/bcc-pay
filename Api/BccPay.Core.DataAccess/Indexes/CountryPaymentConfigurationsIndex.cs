using System.Linq;
using BccPay.Core.Domain;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using Raven.Client.Documents.Indexes;

namespace BccPay.Core.DataAccess.Indexes
{
    public class CountryPaymentConfigurationsIndex : AbstractMultiMapIndexCreationTask<CountryPaymentConfigurationsIndex.Result>
    {
        public CountryPaymentConfigurationsIndex()
        {
            AddMap<Country>(countries => from country in countries
                                         from paymentConfig in country.PaymentConfigurations
                                         from configId in paymentConfig.PaymentConfigurationIds
                                         let config = LoadDocument<PaymentConfiguration>("payment-configurations/" + configId)
                                         select new Result
                                         {
                                             ConfigId = configId,
                                             Condition = paymentConfig.Condition,
                                             CountryCode = country.CountryCode,
                                             Currency = config.Settings.Currency,
                                             PaymentMethod = config.Settings.PaymentMethod,
                                             PaymentProvider = config.Provider
                                         });

            Store(x => x.ConfigId, FieldStorage.Yes);
            Store(x => x.CountryCode, FieldStorage.Yes);
            Store(x => x.Currency, FieldStorage.Yes);
            Store(x => x.PaymentMethod, FieldStorage.Yes);
            Store(x => x.PaymentProvider, FieldStorage.Yes);
            Store(x => x.Condition, FieldStorage.Yes);
        }

        public class Result
        {
            public string ConfigId { get; set; }

            public string Condition { get; set; }

            public string CountryCode { get; set; }

            public Currency Currency { get; set; }

            public PaymentMethod PaymentMethod { get; set; }

            public PaymentProvider PaymentProvider { get; set; }
        }
    }
}
