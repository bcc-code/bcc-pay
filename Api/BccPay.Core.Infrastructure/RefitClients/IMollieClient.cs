using System.Threading.Tasks;
using BccPay.Core.Infrastructure.PaymentModels.Request.Mollie;
using BccPay.Core.Infrastructure.PaymentModels.Response.Mollie;
using Refit;

namespace BccPay.Core.Infrastructure.RefitClients
{
    public interface IMollieClient
    {
        [Post("/v2/payments")]
        public Task<MolliePaymentResponse> CreatePayment(MolliePaymentRequest paymentRequest);
    }
}
