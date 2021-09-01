using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BccPay.Core.Domain;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.Configuration;
using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;

namespace BccPay.Core.Cqrs.Queries
{
    public record GetCountryPaymentConfigurationsQuery(
        string[] CountryCodes,
        string[] PaymentTypes = null) : IRequest<List<PaymentConfigurationResult>>;

    public class PaymentConfigurationResult
    {
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentProvider Provider { get; set; }
        public Currency Currency { get; set; }
        public string[] PaymentConfigurationId { get; set; }

    }

    public class GetCountryPaymentConfigurationsQueryHandler : IRequestHandler<GetCountryPaymentConfigurationsQuery, List<PaymentConfigurationResult>>
    {
        private readonly IAsyncDocumentSession _documentSession;

        public GetCountryPaymentConfigurationsQueryHandler(IAsyncDocumentSession documentSession)
        {
            _documentSession = documentSession
                ?? throw new ArgumentNullException(nameof(documentSession));
        }

        public async Task<List<PaymentConfigurationResult>> Handle(GetCountryPaymentConfigurationsQuery request, CancellationToken cancellationToken)
        {
            var configurations = await _documentSession.Query<PaymentConfiguration>()
                                    .Where(paymentConfiguration
                                        => request.CountryCodes.ContainsAny(paymentConfiguration.Conditions.CountryCodes))
                                    //.SelectMany(x => x.PaymentProviderDefinitionIds)
                                    .ToListAsync(cancellationToken);
            //|| paymentConfiguration.Conditions.CountryCodes.Where(value => value == Country.DefaultCountryCode).Any()


            //var paymentProviderDefinition = await _documentSession.Query<PaymentProviderDefinition>()
            //         .Where(definition => configurations.Contains(definition.PaymentConfigurationCode))
            //         .Select(definition => new PaymentConfigurationResult
            //         {
            //             Currency = definition.Settings.Currency,
            //             PaymentMethod = definition.Settings.PaymentMethod,
            //             Provider = definition.Provider
            //         })
            //         .ToListAsync(cancellationToken);

            return null;
            //if (configurations.Count(c => c.CountryCode == Country.DefaultCountryCode) != 1)
            //{
            //    throw new InvalidConfigurationException("There must be one payment configuration with the 'default' country code");
            //}
            //
            //// filter configurations by conditions
            //bool anyCondition = request.Conditions?.Any() ?? false;
            //if (anyCondition)
            //{
            //    configurations = FilterConfigurations(configurations, request.Conditions);
            //}
            //
            //return configurations
            //    .Select(x => new PaymentConfigurationResult { Id = x.ConfigId, Currency = x.Currency, PaymentMethod = x.PaymentMethod, Provider = x.PaymentProvider })
            //    .DistinctBy(x => x.Id)
            //    .ToList();
        }

        //private static List<CountryPaymentConfigurationsIndex.Result> FilterConfigurations(List<CountryPaymentConfigurationsIndex.Result> configurations, IEnumerable<KeyValuePair<string, string>> conditions)
        //{
        //    return configurations.Where(config =>
        //    {
        //        if (string.IsNullOrEmpty(config.Type))
        //        {
        //            return true;
        //        }
        //
        //        IEnumerable<KeyValuePair<string, string>> configConditions = config.Type.Split('&').Select(condition =>
        //        {
        //            var pair = condition.Split('=');
        //            return new KeyValuePair<string, string>(pair[0], pair[1]);
        //        });
        //
        //        var conditionsMet = conditions.All(c =>
        //            configConditions.Any(
        //                x => string.Equals(x.Key, c.Key) && string.Equals(x.Value, c.Value)));
        //
        //        return conditionsMet;
        //    }).ToList();
        //}
    }
}
