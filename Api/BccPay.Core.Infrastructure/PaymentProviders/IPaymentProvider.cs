using BccPay.Core.Infrastructure.Dtos;
using System.Threading.Tasks;

namespace BccPay.Core.Infrastructure.PaymentProviders
{
    public interface IPaymentProvider
    {
        public string PaymentMethod { get; }
        Task<object> CreatePayment(PaymentRequestDto paymentRequest);
    }
}
