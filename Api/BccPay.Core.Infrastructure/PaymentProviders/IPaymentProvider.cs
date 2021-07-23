using BccPay.Core.Infrastructure.Dtos;
using System.Threading.Tasks;

namespace BccPay.Core.Infrastructure.PaymentProviders
{
    public interface IPaymentProvider
    {
        public string PaymentMethod { get; }

        Task<string> CreatePayment(PaymentRequestDto paymentRequest);

        //Task<PaymentResponse> GetPayment(string paymentId);
    }
}
