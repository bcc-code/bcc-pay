using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BccPay.Core.Domain;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.BccPaymentSettings;
using BccPay.Core.Infrastructure.Constants;
using BccPay.Core.Infrastructure.Dtos;
using BccPay.Core.Infrastructure.Helpers;
using BccPay.Core.Infrastructure.PaymentModels.Response.Mollie;
using BccPay.Core.Infrastructure.PaymentProviders.RequestBuilders;
using BccPay.Core.Infrastructure.PaymentProviders.RequestBuilders.Implementations;
using BccPay.Core.Infrastructure.RefitClients;
using Refit;

namespace BccPay.Core.Infrastructure.PaymentProviders.Implementations.Mollie;

internal class MolliePaymentProvider : IPaymentProvider
{
    private readonly IMollieClient _mollieClient;
    private readonly ICurrencyService _currencyService;
    private readonly MollieProviderOptions _options;

    public MolliePaymentProvider(MollieProviderOptions options,
        IMollieClient mollieClient,
        ICurrencyService currencyService)
    {
        _mollieClient = mollieClient
                        ?? throw new ArgumentNullException(nameof(mollieClient));
        _currencyService = currencyService
                           ?? throw new ArgumentNullException(nameof(currencyService));
        _options = options;
    }

    public PaymentProvider PaymentProvider => PaymentProvider.Mollie;

    public async Task<AttemptCancellationResult> TryCancelPreviousPaymentAttempt(Attempt attempt)
    {
        var details = (MollieStatusDetails)attempt.StatusDetails;
        var paymentResult = (MollieGetPaymentResponse)await GetPayment(details.MolliePaymentId);

        if (paymentResult.Status == PaymentProviderConstants.Mollie.Webhook.Paid)
        {
            attempt.AttemptStatus = AttemptStatus.PaidSucceeded;

            return AttemptCancellationResult.AlreadyCompleted;
        }
        else
        {
            attempt.AttemptStatus = AttemptStatus.Canceled;

            var mollieCancelDetails = await CancelPayment(details);

            return mollieCancelDetails.IsSuccess
                ? AttemptCancellationResult.SuccessfullyCancelled
                : AttemptCancellationResult.ProviderFailedCancellation;
        }
    }

    private async Task<IStatusDetails> CancelPayment(MollieStatusDetails statusDetails)
    {
        try
        {
            var result = await _mollieClient.CancelPaymentAsync(statusDetails.MolliePaymentId);

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                statusDetails.IsSuccess = true;
                return statusDetails;
            }

            if (statusDetails.Errors is null)
            {
                statusDetails.Errors = new List<string>
                {
                    result.Error?.Content?.ToString()
                };
            }
            else
            {
                statusDetails.Errors.Add(result.Error?.Content?.ToString());
            }

            statusDetails.IsSuccess = false;
            return statusDetails;
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
        CurrencyConversionRecord currencyConversion = null;

        if (settings.PaymentMethod is PaymentMethod.Giropay or PaymentMethod.iDeal)
        {
            if (paymentRequest.Ticket is null)
            {
                currencyConversion = await _currencyService.Exchange(
                    paymentRequest.BaseCurrency,
                    paymentRequest.OtherCurrency,
                    paymentRequest.Amount,
                    settings.Markup);
            }
            else
            {
                var ticket = paymentRequest.Ticket;
                currencyConversion = new CurrencyConversionRecord(
                    ticket.Updated,
                    ticket.BaseCurrency,
                    ticket.OtherCurrency,
                    ticket.ExchangeRate,
                    ticket.BaseCurrencyAmount.GetValueOrDefault(),
                    ticket.OtherCurrencyAmount.GetValueOrDefault());
            }

            paymentRequest.Amount = currencyConversion.ToAmount;
        }

        IMollieRequestBuilder requestBuilder = CreateRequestBuilder(settings);
        var request = requestBuilder.BuildMolliePaymentRequest(paymentRequest, settings.PaymentMethod);

        try
        {
            var paymentResult = await _mollieClient.CreatePayment(request);

            return new MollieStatusDetails
            {
                MolliePaymentId = paymentResult.Id,
                CheckoutUrl = paymentResult.Links?.Checkout?.Href,
                Description = paymentResult.Description,
                ExpiresAt = paymentResult.ExpiresAt,
                CurrencyConversionResult = currencyConversion,
                WebhookUrl = paymentResult.WebhookUrl,
                IsSuccess = true
            };
        }
        catch (ApiException exception)
        {
            return new MollieStatusDetails
            {
                IsSuccess = false,
                Errors = new List<string> { exception.Content }
            };
        }
    }

    public async Task<IPaymentResponse> GetPayment(string paymentId)
    {
        try
        {
            var result = await _mollieClient.GetPaymentInformation(paymentId);
            result.IsSuccess = true;
            return result;
        }
        catch (ApiException exception)
        {
            return new MollieGetPaymentResponse
            {
                IsSuccess = false,
                Error = exception.Content
            };
        }
    }

    private IMollieRequestBuilder CreateRequestBuilder(PaymentSetting settings)
    {
        // NOTE: probably this part is useless and need to remove (giropay and ideal have the same request build options)
        return settings switch
        {
            { PaymentMethod: PaymentMethod.Giropay } => new MollieRequestBuilder(_options),
            { PaymentMethod: PaymentMethod.iDeal } => new MollieRequestBuilder(_options),
            _ => throw new NotImplementedException()
        };
    }

    public Task ChargePayment(Payment payment, Attempt attempt)
    {
        throw new NotImplementedException();
    }
}
