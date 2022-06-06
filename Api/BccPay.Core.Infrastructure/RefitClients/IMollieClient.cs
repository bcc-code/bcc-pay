using System.Threading.Tasks;
using BccPay.Core.Infrastructure.PaymentModels.Request.Mollie;
using BccPay.Core.Infrastructure.PaymentModels.Response.Mollie;
using Refit;

namespace BccPay.Core.Infrastructure.RefitClients;

internal interface IMollieClient
{
    [Post("/v2/payments")]
    Task<MollieCreatePaymentResponse> CreatePayment(MolliePaymentRequest paymentRequest);

    [Get("/v2/payments/{id}")]
    Task<MollieGetPaymentResponse> GetPaymentInformation([AliasAs("id")] string paymentId);

    [Delete("/v2/payments/{id}")]
    Task<ApiResponse<object>> CancelPaymentAsync([AliasAs("id")] string paymentId);
}
