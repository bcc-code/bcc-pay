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
            string normalizedAmount = String.Format("{0:0.00}", paymentRequest.Amount.TwoDigitsAfterPoint());
            return new MolliePaymentRequest
            {
                Amount = new MollieAmount
                {
                    Currency = Currencies.EUR.ToString(),
                    Value = normalizedAmount
                },
                Locale = GetLocale(paymentRequest.AcceptLanguage),
                Method = new string[] {paymentMethod.ToString().ToLower()},
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