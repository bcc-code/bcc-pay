using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using BccPay.Core.Domain;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.BccPaymentSettings;
using BccPay.Core.Infrastructure.Constants;
using BccPay.Core.Infrastructure.Dtos;
using BccPay.Core.Infrastructure.PaymentModels.Request.Nets;
using BccPay.Core.Infrastructure.PaymentModels.Response.Nets;
using BccPay.Core.Infrastructure.PaymentProviders.RequestBuilders;
using BccPay.Core.Infrastructure.PaymentProviders.RequestBuilders.Implementations;
using BccPay.Core.Infrastructure.RefitClients;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Refit;

namespace BccPay.Core.Infrastructure.PaymentProviders.Implementations.Nets;

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

    public async Task<IStatusDetails> CreatePayment(PaymentRequestDto paymentRequest, PaymentSetting settings)
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
                    HostedPaymentPageUrl = result.HostedPaymentPageUrl,
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

    public async Task<IPaymentResponse> GetPayment(string paymentId)
    {
        try
        {
            var result = await _netsClient.RetrievePayment(_headers, paymentId);

            return new NetsGetPaymentResponse
            {
                IsSuccess = true,
                Checkout = result.Payment.Checkout,
                Order = result.Payment.Order,
                Summary = result.Payment.Summary
            };
        }
        catch (ApiException exception)
        {
            return new NetsGetPaymentResponse
            {
                IsSuccess = false,
                Error = exception?.Content
            };
        }
    }

    public async Task ChargePayment(Payment payment, Attempt attempt)
    {
        var netsStatusDetails = (NetsStatusDetails)attempt.StatusDetails;
        var invoice = await _netsClient.ChargePayment(_headers,
             netsStatusDetails.PaymentCheckoutId,
             new NetsChargeRequest
             {
                 Amount = Convert.ToInt32(payment.Amount * 100)
             });

        netsStatusDetails.InvoiceId = invoice?.Invoice?.InvoiceNumber;
    }

    private INetsPaymentRequestBuilder CreateRequestBuilder(PaymentSetting settings)
    {
        // create a builder depending on the settings
        return settings switch
        {
            { PaymentMethod: PaymentMethod.CreditCardOrVipps } => new NetsRequestBuilder(_options),
            _ => throw new NotImplementedException()
        };
    }
}
