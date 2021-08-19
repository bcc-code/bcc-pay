using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BccPay.Core.DataAccess.Indexes;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.Exceptions;
using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace BccPay.Core.Cqrs.Queries
{
    public record GetCountryPaymentConfigurationsQuery(string CountryCode) : IRequest<List<PaymentConfigurationResult>>;

    public class PaymentConfigurationResult
    {
        public string Id { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public PaymentProvider Provider { get; set; }

        public Currency Currency { get; set; }

        public string CountryCode { get; set; }
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
                                    .Select(x => new PaymentConfigurationResult
                                    {
                                        Id = x.ConfigId,
                                        Currency = x.Currency,
                                        PaymentMethod = x.PaymentMethod,
                                        Provider = x.PaymentProvider,
                                        CountryCode = x.CountryCode
                                    })
                                    .ToListAsync(cancellationToken);

            if (configurations.Count(c => c.CountryCode == Country.DefaultCountryCode) != 1)
            {
                throw new InvalidConfigurationException("There must be one payment configuration with the 'default' country code");
            }

            // remove default configuration if we have it explicitly specified for the country
            if (configurations.Count > 1)
            {
                configurations = configurations.Where(x => x.CountryCode != Country.DefaultCountryCode).ToList();
            }

            return configurations;
        }
    }
}
