using System.Linq;
using BccPay.Core.Domain.Entities;
using Raven.Client.Documents.Indexes;

namespace BccPay.Core.DataAccess.Indexes
{
    public class PaymentConfigurationIndex : AbstractMultiMapIndexCreationTask<PaymentConfigurationIndex.Result>
    {
        public PaymentConfigurationIndex()
        {
            AddMapForAll<PaymentConfiguration>(paymentConfigurations => from paymentConfiguration in paymentConfigurations
                                                                        select new Result
                                                                        {
                                                                            PaymentProviderDefinitionIds = paymentConfiguration.PaymentProviderDefinitionIds,
                                                                            CountryCode = paymentConfiguration.CountryCode,
                                                                            Conditions = new PaymentConditionsIndexResult
                                                                            {
                                                                                CurrencyCodes = paymentConfiguration.Conditions.CurrencyCodes,
                                                                                PaymentTypes = paymentConfiguration.Conditions.PaymentTypes
                                                                            },
                                                                            SearchContent = paymentConfiguration.Conditions.CurrencyCodes.Concat(paymentConfiguration.Conditions.PaymentTypes).ToArray()
                                                                        });

            Store(x => x.CountryCode, FieldStorage.Yes);
            Store(x => x.PaymentProviderDefinitionIds, FieldStorage.Yes);
            Store(x => x.Conditions, FieldStorage.Yes);

            Index(x => x.CountryCode, FieldIndexing.Exact);
            Index(x => x.SearchContent, FieldIndexing.Exact);
        }

        public class Result
        {
            public string Id { get; set; }

            public string CountryCode { get; init; }

            public PaymentConditionsIndexResult Conditions { get; set; }

            public string[] PaymentProviderDefinitionIds { get; set; }

            public string[] SearchContent { get; set; }
        }

        public class PaymentConditionsIndexResult
        {
            public string[] PaymentTypes { get; set; }
            public string[] CurrencyCodes { get; set; }
        }
    }
}
