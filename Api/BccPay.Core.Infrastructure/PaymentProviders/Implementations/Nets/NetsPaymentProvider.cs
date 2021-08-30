using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using BccPay.Core.Domain;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.Constants;
using BccPay.Core.Infrastructure.Dtos;
using BccPay.Core.Infrastructure.PaymentModels.Request.Nets;
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

        public async Task<AttemptCancellationResult> TryCancelPreviousPaymentAttempt(Attempt attempt)
        {
            var details = (NetsStatusDetails)attempt.StatusDetails;

            if (details.WebhookStatus == PaymentProviderConstants.Nets.Webhooks.ChargeCreated)
            {
                attempt.AttemptStatus = AttemptStatus.PaidSucceeded;

                return AttemptCancellationResult.AlreadyCompleted;
            }
            else
            {
                attempt.AttemptStatus = AttemptStatus.Canceled;

                var netsCancelDetails = await CancelPayment(details);

                return netsCancelDetails.IsSuccess
                    ? AttemptCancellationResult.SuccessfullyCancelled
                    : AttemptCancellationResult.ProviderFailedCancellation;
            }
        }

        private async Task<IStatusDetails> CancelPayment(NetsStatusDetails statusDetails)
        {
            try
            {
                var providerResult = await _netsClient.TerminatePayment(_headers, statusDetails.PaymentCheckoutId);

                if (providerResult.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    statusDetails.IsSuccess = true;
                    return statusDetails;
                }
                else
                {
                    if (statusDetails.Errors is null)
                    {
                        statusDetails.Errors = new List<string>
                        {
                           await providerResult.Content.ReadAsStringAsync()
                        };
                    }
                    else
                    {
                        statusDetails.Errors.Add(await providerResult.Content.ReadAsStringAsync());
                    }
                    statusDetails.IsSuccess = false;
                    return statusDetails;
                }
            }
            catch (ApiException exception)
            {
                if (statusDetails.Errors is null)
                {
                    statusDetails.Errors = new List<string>
                    {
                        exception.Content?.ToString()
                    };
                }
                else
                {
                    statusDetails.Errors.Add(exception.Content?.ToString());
                }

                statusDetails.IsSuccess = false;
                return statusDetails;
            }
        }

        public async Task<IStatusDetails> CreatePayment(PaymentRequestDto paymentRequest, PaymentSettings settings)
        {
            INetsPaymentRequestBuilder requestBuilder = CreateRequestBuilder(settings);
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

        public async Task ChargePayment(Payment payment, Attempt attempt)
        {
            var netsStatusDetails = (NetsStatusDetails)attempt.StatusDetails;
            try
            {
                var invoice = await _netsClient.ChargePayment(_headers,
                     netsStatusDetails.PaymentCheckoutId,
                     new NetsChargeRequest
                     {
                         Amount = Convert.ToInt32(payment.Amount * 100)
                     });

                netsStatusDetails.InvoiceId = invoice?.Invoice?.InvoiceNumber;
            }
            catch (ApiException exception)
            { }
        }

        private INetsPaymentRequestBuilder CreateRequestBuilder(PaymentSettings settings)
        {
            // create a builder depending on the settings
            return settings switch
            {
                { PaymentMethod: PaymentMethod.CreditCardOrVipps } => new NetsRequestBuilder(_options),
                _ => throw new NotImplementedException()
            };
        }
    }
}
