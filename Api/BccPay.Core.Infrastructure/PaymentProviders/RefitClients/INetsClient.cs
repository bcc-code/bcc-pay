using BccPay.Core.Infrastructure.PaymentModels.Request.Nets;
using BccPay.Core.Infrastructure.PaymentModels.Response.Nets;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BccPay.Core.Infrastructure.PaymentProviders.RefitClients
{
    public interface INetsClient
    {
        [Post("/v1/payments")]
        Task<CreatePaymentResponse> CreatePaymentAsync([HeaderCollection] IDictionary<string, string> headers, [Body] NetsPaymentRequest payment);

        [Get("/v1/payments/{paymentId}")]
        Task<RetrievePaymentResponse> RetrievePayment([HeaderCollection] IDictionary<string, string> headers, [Query] string paymentId);
    }
}

