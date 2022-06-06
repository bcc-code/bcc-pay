using System;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.BccPaymentSettings;
using BccPay.Core.Infrastructure.Dtos;
using BccPay.Core.Infrastructure.PaymentModels.MollieNodes;
using BccPay.Core.Infrastructure.PaymentModels.Request.Mollie;
using BccPay.Core.Shared.Converters;
using Microsoft.AspNetCore.WebUtilities;

namespace BccPay.Core.Infrastructure.PaymentProviders.RequestBuilders.Implementations;

internal class MollieRequestBuilder : BaseRequestBuilder, IMollieRequestBuilder
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
                Currency = Currencies.EUR.ToString(),
                Value =
                    $"{paymentRequest.Ticket?.OtherCurrencyAmount?.ToAmountOfDigitsAfterPoint() ?? paymentRequest.Amount.ToAmountOfDigitsAfterPoint():0.00}"
            },
            Locale = GetLocale(paymentRequest.AcceptLanguage),
            Method = new[] {paymentMethod.ToString().ToLower()},
            Description = paymentRequest.Description,
            RedirectUrl = GetRedirectUrl(
                _options.RedirectUrl,
                paymentRequest.UsePaymentIdAsRouteInRedirectUrl is null ? null : paymentRequest.PaymentId,
            paymentRequest.UsePaymentIdAsRouteInRedirectUrl is null ? null : paymentRequest.AttemptId),
            WebhookUrl = _options.WebhookUrl + $"/{paymentRequest.PaymentId}",
            Links = new { }
        };
    }
}
