using BccPay.Core.Infrastructure.Constants;
using BccPay.Core.Infrastructure.PaymentProviders.RefitClients;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Mime;
using System.Threading.Tasks;

namespace BccPay.Core.Infrastructure.PaymentProviders.Implementations
{
    public class NetsProvider : IPaymentProvider
    {
        private readonly INetsClient _netsClient;
        private readonly IConfiguration _configuration;
        private readonly IDictionary<string, string> _headers;

        public NetsProvider(INetsClient netsClient,
            IConfiguration configuration)
        {
            _netsClient = netsClient
                ?? throw new ArgumentNullException(nameof(netsClient));

            _configuration = configuration;
            _headers = new Dictionary<string, string>
            {
                {"Authorization", _configuration[PaymentProviderConstants.NetsSecretKey] },
                {"content-type", MediaTypeNames.Application.Json }
            };
        }

        public async Task<string> CreatePayment()
        {
            try
            {
                var result = await _netsClient.CreatePaymentAsync(_headers, new PaymentModels.Request.Nets.CreatePaymentRequest());

                return result.PaymentId;
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception);
                throw;
            }
        }
    }
}
