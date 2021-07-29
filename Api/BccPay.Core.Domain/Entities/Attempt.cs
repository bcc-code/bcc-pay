using BccPay.Core.Enums;
using System;

namespace BccPay.Core.Domain.Entities
{
    public class Attempt
    {
        public static string GetPaymentAttemptId(Guid paymentId, Guid paymentAttemptId) =>
             $"payments/{paymentId}/attempts/{paymentAttemptId}";

        public string Id => GetPaymentAttemptId(PaymentId, PaymentAttemptId);
        // TODO: find better way
        public Guid PaymentId { get; set; }

        public Guid PaymentAttemptId { get; set; }
        public bool IsActive { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public DateTime Created { get; set; }
        public AttemptStatus PaymentStatus { get; set; }
        public string PaymentIdForCheckoutForm { get; set; }
        public string CountryCode { get; set; }
    }
}
