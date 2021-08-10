using System;
using System.Threading.Tasks;
using BccPay.Core.Domain;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.Dtos;
using BccPay.Core.Infrastructure.Helpers;
using BccPay.Core.Infrastructure.RefitClients;

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
                                    0.015M);

                request.Amount.Value = currencyConversion.Gross.ToString();
            }

            var paymentResult = await _mollieClient.CreatePayment(request);

            return new MollieStatusDetails
            {
                PaymentId = paymentResult.Id,
                CheckoutUrl = paymentResult.Links?.Checkout?.Href,
                Description = paymentResult.Description,
                ExpiresAt = paymentResult.ExpiresAt,
                CurrencyConversionResult = currencyConversion,
                WebhookUrl = paymentResult.WebhookUrl,
                IsSuccessful = true
            };
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
