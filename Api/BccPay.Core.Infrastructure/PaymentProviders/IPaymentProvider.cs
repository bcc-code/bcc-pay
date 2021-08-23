using System.Threading.Tasks;
using BccPay.Core.Domain;
using BccPay.Core.Domain.Entities;
using BccPay.Core.Enums;
using BccPay.Core.Infrastructure.Dtos;

namespace BccPay.Core.Infrastructure.PaymentProviders
{
    public interface IPaymentProvider
    {
        public PaymentProvider PaymentProvider { get; }

        Task ChargePayment(Payment payment, Attempt attempt);
        Task<IStatusDetails> CreatePayment(PaymentRequestDto paymentRequest, PaymentSettings settings);
        Task<IPaymentResponse> GetPayment(string paymentId);
        Task<AttemptCancellationResult> TryCancelPreviousPaymentAttempt(Attempt attempt);
    }
}
