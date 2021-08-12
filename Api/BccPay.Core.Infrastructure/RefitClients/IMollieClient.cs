using System.Threading.Tasks;
using BccPay.Core.Infrastructure.PaymentModels.Request.Mollie;
using BccPay.Core.Infrastructure.PaymentModels.Response.Mollie;
using Refit;

namespace BccPay.Core.Infrastructure.RefitClients
{
    internal interface IMollieClient
    {
        [Post("/v2/payments")]
        public Task<MollieCreatePaymentResponse> CreatePayment(MolliePaymentRequest paymentRequest);

        [Get("/v2/payments/{id}")]
        public Task<MollieGetPaymentResponse> GetPaymentInformation([AliasAs("id")] string paymentId);
    }
}
