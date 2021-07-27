using BccPay.Core.Enums;
using System;

namespace BccPay.Core.Contracts.Responses
{
    public class PaymentResponse
    {
        public Guid PaymentId { get; set; }
        public string PaymentIdForCheckoutForm { get; set; }
        public Guid PayerId { get; set; }
        public string CurrencyCode { get; set; }
        public decimal Amount { get; set; }
        public string CountryCode { get; set; }

        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
}
