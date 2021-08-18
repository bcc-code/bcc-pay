using System;
using System.Threading.Tasks;
using BccPay.Core.Domain;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.Dtos;
using BccPay.Core.Infrastructure.Helpers;
using BccPay.Core.Infrastructure.PaymentModels.Response.Mollie;
using BccPay.Core.Infrastructure.PaymentProviders.RequestBuilders;
using BccPay.Core.Infrastructure.PaymentProviders.RequestBuilders.Implementations;
using BccPay.Core.Infrastructure.RefitClients;
using Refit;

namespace BccPay.Core.Infrastructure.PaymentProviders.Implementations.Mollie
{
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

        public async Task<bool> CancelPayment(string paymentId)
        {
            try
            {
                var result = await _mollieClient.CancelPaymentAsync(paymentId);
                return result.StatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (ApiException)
            {
                return false;
            }
        }

        public async Task<IStatusDetails> CreatePayment(PaymentRequestDto paymentRequest, PaymentSettings settings)
        {
            IMollieRequestBuilder requestBuilder = CreateRequestBuilder(settings);

            var request = requestBuilder.BuildMolliePaymentRequest(paymentRequest);

            CurrencyConversionRecord currencyConversion = null;
            if (settings.PaymentMethod == PaymentMethod.Giropay)
            {
                currencyConversion = await _currencyService.Exchange(
                                    paymentRequest.Currency,
                                    Currencies.EUR.ToString(),
                                    paymentRequest.Amount,
                                    _options.RateMarkup);

                request.Amount.Value = currencyConversion.ToAmount.ToString();
            }
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
                    Error = exception.Content
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

        private IMollieRequestBuilder CreateRequestBuilder(PaymentSettings settings)
        {
            return settings switch
            {
                { PaymentMethod: PaymentMethod.Giropay } => new MollieGiropayRequestBuilder(_options),
                _ => throw new NotImplementedException()
            };
        }
    }
}
