using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BccPay.Core.Cqrs.Extensions;
using BccPay.Core.DataAccess.Indexes;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.Exceptions;
using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace BccPay.Core.Cqrs.Queries
{
    public record GetCountryPaymentConfigurationsQuery(
        string CountryCode,
        IEnumerable<KeyValuePair<string, string>> Conditions = null) : IRequest<List<PaymentConfigurationResult>>;

    public class PaymentConfigurationResult
    {
        public string Id { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public PaymentProvider Provider { get; set; }

        public Currency Currency { get; set; }
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
            var configurations = await _documentSession.Query<CountryPaymentConfigurationsIndex.Result, CountryPaymentConfigurationsIndex>()
                                    .Where(x => x.CountryCode == request.CountryCode || x.CountryCode == Country.DefaultCountryCode)
                                    .Select(x => new CountryPaymentConfigurationsIndex.Result
                                    {
                                        ConfigId = x.ConfigId,
                                        Condition = x.Condition,
                                        Currency = x.Currency,
                                        PaymentMethod = x.PaymentMethod,
                                        PaymentProvider = x.PaymentProvider,
                                        CountryCode = x.CountryCode
                                    })
                                    .ToListAsync(cancellationToken);

            if (configurations.Count(c => c.CountryCode == Country.DefaultCountryCode) != 1)
            {
                throw new InvalidConfigurationException("There must be one payment configuration with the 'default' country code");
            }

            // filter configurations by conditions
            bool anyCondition = request.Conditions?.Any() ?? false;
            if (anyCondition)
            {
                configurations = FilterConfigurations(configurations, request.Conditions);
            }

            return configurations
                .Select(x => new PaymentConfigurationResult { Id = x.ConfigId, Currency = x.Currency, PaymentMethod = x.PaymentMethod, Provider = x.PaymentProvider })
                .DistinctBy(x => x.Id)
                .ToList();
        }

        private static List<CountryPaymentConfigurationsIndex.Result> FilterConfigurations(List<CountryPaymentConfigurationsIndex.Result> configurations, IEnumerable<KeyValuePair<string, string>> conditions)
        {
            return configurations.Where(config =>
            {
                if (string.IsNullOrEmpty(config.Condition))
                {
                    return true;
                }

                IEnumerable<KeyValuePair<string, string>> configConditions = config.Condition.Split('&').Select(condition =>
                {
                    var pair = condition.Split('=');
                    return new KeyValuePair<string, string>(pair[0], pair[1]);
                });

                var conditionsMet = conditions.All(c =>
                    configConditions.Any(
                        x => string.Equals(x.Key, c.Key) && string.Equals(x.Value, c.Value)));

                return conditionsMet;
            }).ToList();
        }
    }
}
