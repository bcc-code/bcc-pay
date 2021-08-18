using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using BccPay.Core.Domain;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.Dtos;
using BccPay.Core.Infrastructure.PaymentProviders.RequestBuilders;
using BccPay.Core.Infrastructure.PaymentProviders.RequestBuilders.Implementations;
using BccPay.Core.Infrastructure.RefitClients;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Refit;

namespace BccPay.Core.Infrastructure.PaymentProviders.Implementations
{
    internal class NetsPaymentProvider : IPaymentProvider
    {
        private readonly INetsClient _netsClient;
        private readonly NetsProviderOptions _options;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDictionary<string, string> _headers;

        public NetsPaymentProvider(INetsClient netsClient, NetsProviderOptions options, IHttpContextAccessor httpContextAccessor)
        {
            _netsClient = netsClient
                ?? throw new ArgumentNullException(nameof(netsClient));

            _options = options;
            _httpContextAccessor = httpContextAccessor;
            _headers = new Dictionary<string, string>
            {
                { HeaderNames.Authorization, _options.SecretKey },
                { HeaderNames.ContentType, MediaTypeNames.Application.Json }
            };
        }

        public PaymentProvider PaymentProvider => PaymentProvider.Nets;

        public async Task<IStatusDetails> CancelPayment(IStatusDetails statusDetails)
        {
            var netsStatusDetails = (NetsStatusDetails)statusDetails;

            try
            {
                var result = await _netsClient.TerminatePayment(_headers, netsStatusDetails.PaymentCheckoutId);

                if (result.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    netsStatusDetails.IsSuccess = true;
                    return netsStatusDetails;
                }

                if (netsStatusDetails.Errors is null)
                {
                    netsStatusDetails.Errors = new List<string>
                    {
                        result.Error?.ToString()
                    };
                }
                else
                {
                    netsStatusDetails.Errors.Add(result.Error?.ToString());
                }

                netsStatusDetails.IsSuccess = false;
                return netsStatusDetails;
            }
            catch (ApiException exception)
            {
                if (netsStatusDetails.Errors is null)
                {
                    netsStatusDetails.Errors = new List<string>
                    {
                        exception.Content?.ToString()
                    };
                }
                else
                {
                    netsStatusDetails.Errors.Add(exception.Content?.ToString());
                }

                netsStatusDetails.IsSuccess = false;
                return netsStatusDetails;
            }
        }

        public async Task<IStatusDetails> CreatePayment(PaymentRequestDto paymentRequest, PaymentSettings settings)
        {
            INetsPaymentRequestBuilder requestBuilder = this.CreateRequestBuilder(settings);
            var referer = new Uri(_httpContextAccessor.HttpContext.Request.Headers["Referer"].ToString());
            var host = $"{referer.Scheme}://{referer.Authority}";

            try
            {
                var result = await _netsClient.CreatePaymentAsync(_headers, requestBuilder.BuildNetsPaymentRequest(paymentRequest, host));

                return new NetsStatusDetails
                {
                    IsSuccess = true,
                    PaymentCheckoutId = result.PaymentId
                };
            }
            catch (ApiException retryException)
            {
                try
                {
                    var result = await _netsClient.CreatePaymentAsync(_headers, requestBuilder.BuildNetsPaymentRequest(paymentRequest, host, isUserDataValid: false));
                    return new NetsStatusDetails
                    {
                        IsSuccess = true,
                        PaymentCheckoutId = result.PaymentId,
                        Errors = new List<string> { "{\"notValidUserBillingDataInTheSystem\":" + retryException?.Content + "}" }
                    };
                }
                catch (ApiException exception)
                {
                    return new NetsStatusDetails
                    {
                        IsSuccess = false,
                        Errors = new List<string> { exception?.Content }
                    };
                }
            }
        }

        public Task<IPaymentResponse> GetPayment(string paymentId)
        {
            throw new NotImplementedException();
        }

        private INetsPaymentRequestBuilder CreateRequestBuilder(PaymentSettings settings)
        {
            // create a builder depending on the settings
            return settings switch
            {
                { PaymentMethod: PaymentMethod.CreditCard } => new NetsCreditCardRequestBuilder(_options),
                _ => throw new NotImplementedException()
            };
        }
    }
}
