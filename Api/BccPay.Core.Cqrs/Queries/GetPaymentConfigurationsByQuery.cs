using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BccPay.Core.Domain.Entities;
using MediatR;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;

namespace BccPay.Core.Cqrs.Queries
{
    public record GetPaymentConfigurationsByQuery(
        string CountryCode,
        string PaymentType = null,
        string CurrencyCode = null) : IRequest<AvailableConfigurationsResult>;

    public record AvailableConfigurationsResult(List<PaymentConfigurationResult> PaymentConfigurations);

    public class PaymentConfigurationResult
    {
        public string CountryCode { get; set; }
        public string[] PaymentProviderDefinitionIds { get; set; }
        public string[] PaymentTypes { get; set; }
        public string[] CurrencyCodes { get; set; }
    }

    public class GetPaymentConfigurationsByQueryHandler : IRequestHandler<GetPaymentConfigurationsByQuery, AvailableConfigurationsResult>
    {
        private readonly IAsyncDocumentSession _documentSession;

        public GetPaymentConfigurationsByQueryHandler(IAsyncDocumentSession documentSession)
        {
            _documentSession = documentSession
                ?? throw new ArgumentNullException(nameof(documentSession));
        }

        public async Task<AvailableConfigurationsResult> Handle(GetPaymentConfigurationsByQuery request, CancellationToken cancellationToken)
        {
            var query = _documentSession.Query<PaymentConfiguration>()
                        .Where(paymentConfiguration
                            => paymentConfiguration.CountryCode.In(new string[] { request.CountryCode, Country.DefaultCountryCode }));

            if (!string.IsNullOrWhiteSpace(request.PaymentType))
                query = query.Where(paymentConfiguration => paymentConfiguration.Conditions.PaymentTypes.Contains(request.PaymentType));
            if (!string.IsNullOrWhiteSpace(request.CurrencyCode))
                query = query.Where(paymentConfiguration => paymentConfiguration.Conditions.CurrencyCodes.Contains(request.CurrencyCode));

            List<PaymentConfigurationResult> paymentConfigurations = new();

            paymentConfigurations = await query.Select(paymentConfiguration
                    => new PaymentConfigurationResult
                    {
                        CountryCode = paymentConfiguration.CountryCode,
                        CurrencyCodes = paymentConfiguration.Conditions.CurrencyCodes,
                        PaymentTypes = paymentConfiguration.Conditions.PaymentTypes,
                        PaymentProviderDefinitionIds = paymentConfiguration.PaymentProviderDefinitionIds
                    }).ToListAsync(cancellationToken);

            return new AvailableConfigurationsResult(paymentConfigurations);
        }
    }
}
