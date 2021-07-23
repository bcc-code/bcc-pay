using BccPay.Core.Enums;

namespace BccPay.Core.Contracts.Responses
{
    public class RetrievePaymentResponse
    {
        public string PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Created { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
    }
}
