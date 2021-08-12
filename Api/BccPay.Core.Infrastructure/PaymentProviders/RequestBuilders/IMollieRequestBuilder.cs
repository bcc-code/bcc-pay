using BccPay.Core.Infrastructure.Dtos;
using BccPay.Core.Infrastructure.PaymentModels.Request.Mollie;

namespace BccPay.Core.Infrastructure.PaymentProviders.RequestBuilders
{
    internal interface IMollieRequestBuilder
    {
        MolliePaymentRequest BuildMolliePaymentRequest(PaymentRequestDto paymentRequest);
    }
}
