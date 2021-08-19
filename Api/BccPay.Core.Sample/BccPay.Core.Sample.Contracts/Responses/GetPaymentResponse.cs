using BccPay.Core.Enums;
using System;
using System.Collections.Generic;

namespace BccPay.Core.Sample.Contracts.Responses
{
    public class GetPaymentResponse
    {
        public Guid PaymentId { get; set; }
        public string PayerId { get; set; }
        public string CurrencyCode { get; set; }
        public string CountryCode { get; set; }
        public decimal Amount { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public List<AttemptResponseModel> Attempts { get; set; }
    }

    public class AttemptResponseModel
    {
        public Guid PaymentAttemptId { get; set; }
        public bool IsActive { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public DateTime Created { get; set; }
        public AttemptStatus PaymentStatus { get; set; }
        public object StatusDetails { get; set; }
    }
}
