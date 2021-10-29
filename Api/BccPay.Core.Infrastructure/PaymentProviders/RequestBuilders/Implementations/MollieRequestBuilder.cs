using System;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.BccPaymentSettings;
using BccPay.Core.Infrastructure.Dtos;
using BccPay.Core.Infrastructure.PaymentModels.MollieNodes;
using BccPay.Core.Infrastructure.PaymentModels.Request.Mollie;
using BccPay.Core.Shared.Converters;

namespace BccPay.Core.Infrastructure.PaymentProviders.RequestBuilders.Implementations
{
    internal class MollieRequestBuilder : IMollieRequestBuilder
    {
        private readonly MollieProviderOptions _options;

        public MollieRequestBuilder(MollieProviderOptions options)
        {
            _options = options;
        }

        public MolliePaymentRequest BuildMolliePaymentRequest(PaymentRequestDto paymentRequest,
            PaymentMethod paymentMethod)
        {
            return new MolliePaymentRequest
            {
                Amount = new MollieAmount
                {
                    Currency = paymentRequest.Ticket?.OtherCurrency.ToString() ?? Currencies.EUR.ToString(), // TODO: change default value
                    Value = $"{paymentRequest.Ticket?.OtherCurrencyAmount?.TwoDigitsAfterPoint() ?? paymentRequest.Amount.TwoDigitsAfterPoint()}"
                },
                Locale = GetLocale(paymentRequest.AcceptLanguage),
                Method = new[] {paymentMethod.ToString().ToLower()},
                Description = paymentRequest.Description,
                RedirectUrl = GetRedirectUrl(paymentRequest.IsMobile),
                WebhookUrl = _options.WebhookUrl + $"/{paymentRequest.PaymentId}",
                Links = new { }
            };
        }

        private string GetRedirectUrl(bool isMobile)
            => isMobile ? _options.RedirectUrlMobileApp : _options.RedirectUrl;

        private static string GetLocale(string browserLanguage)
        {
            if (browserLanguage != "de-DE" && browserLanguage != "nb-NO")
                return "en-US";
            return browserLanguage;
        }
    }
}