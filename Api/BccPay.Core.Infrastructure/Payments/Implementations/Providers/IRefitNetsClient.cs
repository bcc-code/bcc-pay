using BccPay.Core.Infrastructure.Payments.Implementations.Providers.Nets.RequestModels;
using BccPay.Core.Infrastructure.Payments.Implementations.Providers.Nets.ResponseModels;
using Refit;
using System.Threading.Tasks;

namespace BccPay.Core.Infrastructure.Payments.Implementations.Providers
{
    public interface IRefitNetsClient
    {
        [Post("/v1/payments")]
        Task<CreatePaymentResponse> CreatePaymentAsync([Body] CreatePaymentRequest payment);

        [Get("/v1/payments/{paymentId}")]
        Task<RetrievePaymentResponse> RsetrievePayment([Query] string paymentId);
    }
}
