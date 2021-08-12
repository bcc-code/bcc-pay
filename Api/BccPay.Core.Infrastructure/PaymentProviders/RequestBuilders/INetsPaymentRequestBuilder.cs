using BccPay.Core.Infrastructure.Dtos;
using BccPay.Core.Infrastructure.PaymentModels.Request.Nets;

namespace BccPay.Core.Infrastructure.PaymentProviders.RequestBuilders
{
    internal interface INetsPaymentRequestBuilder
    {
        NetsPaymentRequest BuildNetsPaymentRequest(PaymentRequestDto paymentRequest, string originUrl, bool isUserDataValid = true);
    }
}
