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
                                         from configId in country.PaymentConfigurations
                                         let config = LoadDocument<PaymentConfiguration>("payment-configurations/" + configId)
                                         select new Result
                                         {
                                             Id = configId,
                                             CountryCode = country.CountryCode,
                                             Currency = config.Settings.Currency,
                                             PaymentMethod = config.Settings.PaymentMethod,
                                             PaymentProvider = config.Provider
                                         });
        }

        public class Result
        {
            public string Id { get; set; }

            public string CountryCode { get; set; }

            public Currency Currency { get; set; }

            public PaymentMethod PaymentMethod { get; set; }

            public PaymentProvider PaymentProvider { get; set; }
        }
    }
}
