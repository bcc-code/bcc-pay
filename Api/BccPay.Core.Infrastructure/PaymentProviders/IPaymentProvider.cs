using BccPay.Core.Domain.Entities;
using BccPay.Core.Infrastructure.Dtos;
using System.Threading.Tasks;

namespace BccPay.Core.Infrastructure.PaymentProviders
{
    public interface IPaymentProvider
    {
        public string PaymentMethod { get; }
        Task<IStatusDetails> CreatePayment(PaymentRequestDto paymentRequest);
    }
}
