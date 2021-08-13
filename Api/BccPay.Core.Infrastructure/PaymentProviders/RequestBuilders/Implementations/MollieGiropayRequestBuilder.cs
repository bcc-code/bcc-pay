﻿using System;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.Dtos;
using BccPay.Core.Infrastructure.PaymentModels.MollieNodes;
using BccPay.Core.Infrastructure.PaymentModels.Request.Mollie;

namespace BccPay.Core.Infrastructure.PaymentProviders.RequestBuilders.Implementations
{
    internal class MollieGiropayRequestBuilder : IMollieRequestBuilder
    {
        private readonly MollieProviderOptions _options;
        public MollieGiropayRequestBuilder(MollieProviderOptions options)
        {
            _options = options;
        }

        public MolliePaymentRequest BuildMolliePaymentRequest(PaymentRequestDto paymentRequest)
        {
            return new MolliePaymentRequest
            {
                Amount = new MollieAmount
                {
                    Currency = Currencies.EUR.ToString(),
                    Value = String.Format("{0:0.00}", paymentRequest.Amount)
                },
                Locale = GetLocale(paymentRequest.AcceptLanguage),
                Method = new string[] { PaymentMethod.Giropay.ToString().ToLower() },
                Description = paymentRequest.Description,
                RedirectUrl = _options.RedirectUrl,
                WebhookUrl = _options.WebhookUrl + $"/{paymentRequest.PaymentId}",
                Links = new { }
            };
        }

        private string GetLocale(string browserLanguage)
        {
            if (browserLanguage != "de-DE" && browserLanguage != "nb-NO")
                return "en-US";
            return browserLanguage;
        }
    }
}