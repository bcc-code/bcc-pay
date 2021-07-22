using BccPay.Core.Enums;
using System;

namespace BccPay.Core.Domain.Entities
{
    public class Payment
    {
        public Guid Id { get; set; }

        public string PaymentId { get; set; }
        public Guid PayerId { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public string Country { get; set; }

        public DateTime Created => DateTime.Now;
        public PaymentStatus PaymentStatus { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentDetails PaymentDetails { get; set; }
    }
}
