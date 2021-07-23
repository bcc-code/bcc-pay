using System;

namespace BccPay.Core.Contracts.Requests
{
    public class CreatePaymentRequest
    {
        public Guid PayerId { get; set; }
        public string Currency { get; set; }
        public string Country { get; set; }
        public decimal Amount { get; set; }

        public string PaymentMethod { get; set; }
    }
}
