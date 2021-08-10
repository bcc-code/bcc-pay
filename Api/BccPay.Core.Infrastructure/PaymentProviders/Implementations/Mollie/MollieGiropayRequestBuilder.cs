using System;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.Dtos;
using BccPay.Core.Infrastructure.PaymentModels.MollieNodes;
using BccPay.Core.Infrastructure.PaymentModels.Request.Mollie;

namespace BccPay.Core.Infrastructure.PaymentProviders.Implementations.Mollie
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
                    Value = paymentRequest.Amount.ToString()
                },
                Locale = "de_DE",
                Method = new string[] { PaymentMethod.Giropay.ToString().ToLower() },
                Description = $"{paymentRequest.FirstName}-{paymentRequest.LastName}-{Guid.NewGuid()}",
                RedirectUrl = _options.RedirectUrl,
                WebhookUrl = _options.WebhookUrl,
                Links = new { }
            };
        }
    }
}
