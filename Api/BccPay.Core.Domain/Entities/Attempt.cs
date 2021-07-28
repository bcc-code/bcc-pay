using BccPay.Core.Enums;
using System;

namespace BccPay.Core.Domain.Entities
{
    public class Attempt
    {
        public Guid PaymentAttemptId { get; set; }
        public bool IsActive { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public DateTime Created { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string PaymentIdForCheckoutForm { get; set; }
        public string CountryCode { get; set; }
    }
}
